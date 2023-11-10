using Assets.Code.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "PlaneMaster", menuName = "Plane Master", order = 1)]
public class PlaneMaster : ScriptableObject
{

    /// <summary>
    /// List of all currently active render textures
    /// </summary>
    public static List<CustomRenderTexture> activeRenderTextures = new List<CustomRenderTexture>();

    [Tooltip("The name of the plane to use.")]
    public string planeName;

    [Tooltip("The layer of the plane, lower layers will be rendered first.")]
    public int planeLayer;

    [Tooltip("The backdrop of this plane.")]
    public Color backdrop = new Color(0, 0, 0, 0);

    [Tooltip("Does this plane draw to the screen?")]
    public bool drawToScreen = true;

    [Tooltip("How should other planes be drawn on top of this plane?")]
    public PlaneRenderRelay[] renderRelays;

    [SerializeField]
    [Tooltip("The colour matrix of this plane.")]
    private Matrix4x4 colourMatrix = Matrix4x4.identity;

    // We need this because we don't want to update the scriptable object dynamically
    private Matrix4x4 dynamicColourMatrix;

    public Matrix4x4 ColourMatrix
    {
        get => dynamicColourMatrix;
        set {
			dynamicColourMatrix = value;
			_output.material?.SetMatrix("_ColourMatrix", dynamicColourMatrix);
		}
    }

	[Tooltip("The constant part of the colouring for this colour matrix.")]
    public Color colourConstantPart = new Color(0, 0, 0, 0);

	/// <summary>
	/// Z axis that we are assigned to
	/// </summary>
	public int AssignedZ { get; internal set; } = 0;

    [NonSerialized]
    private bool renderingInitialised = false;

	[NonSerialized]
	private CustomRenderTexture _input;
	[NonSerialized]
	private CustomRenderTexture _output;

    /// <summary>
    /// We need to maintain references to the materials to prevent them from being GC'd betwene levels?
    /// I have no idea why this is needed, most likely because custom render textures aren't working within C#
    /// but within the underlying unity engine code meaning that there are no references maintained between
    /// levels.
    /// </summary>
    private static Queue<Material> materialReferenceList = new Queue<Material>();

	public CustomRenderTexture InputRenderTexture
    {
        get
        {
            if (_input == null)
            {
                throw new Exception("Plane master rendering has not been initialised.");
			}
            return _input;
		}
    }

    public CustomRenderTexture OutputRenderTexture
    {
        get
        {
			if (_output == null)
			{
				throw new Exception("Plane master rendering has not been initialised.");
			}
			return _output;
		}
    }

    internal void InitialiseRendering(PlaneMasterController parent)
    {
		if (renderingInitialised)
            return;
		renderingInitialised = true;
        dynamicColourMatrix = colourMatrix;
		_input = new CustomRenderTexture(ResolutionController.Width, ResolutionController.Height, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        activeRenderTextures.Add(_input);
		_input.filterMode = FilterMode.Point;
        _input.Create();
		// We need this to prevent the artifacts
		Debug.Log($"Created input render texture for the {planeName} plane.");
        if (renderRelays == null || renderRelays.Length == 0)
        {
            _output = _input;
            return;
        }
        CustomRenderTexture above = _input;
        CustomRenderTexture next = _input;
        foreach (PlaneRenderRelay renderRelay in renderRelays.OrderBy(x => x.incomingPlane.planeLayer))
        {
            next = new CustomRenderTexture(ResolutionController.Width, ResolutionController.Height, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
			next.Create();

			activeRenderTextures.Add(next);

			// We need this to prevent the artifacts
			if (renderRelay.overrideMaterial != null)
            {
                next.initializationMaterial = new Material(renderRelay.overrideMaterial);
            }
            else
            {
                switch (renderRelay.drawMode)
                {
                    case PlaneDrawMode.ADDITIVE:
                        next.initializationMaterial = new Material(parent.additiveMaterial);
                        break;
                    case PlaneDrawMode.MULTIPLICATIVE:
                        next.initializationMaterial = new Material(parent.multiplicativeMaterial);
                        break;
                    case PlaneDrawMode.ALPHA_MASK:
                        next.initializationMaterial = new Material(parent.alphaMaskMaterial);
                        break;
                    default:
                        next.initializationMaterial = new Material(parent.defaultMaterial);
                        break;
                }
			}
			// We need this between scenes
            materialReferenceList.Enqueue(next.initializationMaterial);
			renderRelay.incomingPlane.InitialiseRendering(parent);
			next.initializationMaterial.SetTexture("_SourceTexture", above);
			next.initializationMaterial.SetTexture("_TargetTexture", renderRelay.incomingPlane.OutputRenderTexture);
            next.initializationMaterial.SetMatrix("_ColourMatrix", dynamicColourMatrix);
            next.initializationMaterial.SetVector("_ColourMatrixConstant", colourConstantPart);

            if (renderRelay.dontClear)
            {
                next.doubleBuffered = true;
                next.updateMode = CustomRenderTextureUpdateMode.Realtime;
                next.updatePeriod = 0;
                // Reset the texture on scene load
                SceneManager.sceneLoaded += (scene, mode) =>
                {
                    RenderTexture.active = next;
                    GL.Clear(true, true, Color.clear);
				};
            }

            next.filterMode = FilterMode.Point;

			next.material = next.initializationMaterial;
            next.initializationSource = CustomRenderTextureInitializationSource.Material;
			next.initializationMode = CustomRenderTextureUpdateMode.Realtime;
			renderRelay.assignedRenderTexture = next;
			next.Initialize();
			next.Update();
			above = next;
		}
        // Set the final output to teh last in the chain of render relays
        _output = next;
	}

}
