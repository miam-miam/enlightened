using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Code.Rendering
{
	[System.Serializable]
	public class PlaneRenderRelay
	{

		[Tooltip("The plane to be rendered on top of the target plane")]
		public PlaneMaster incomingPlane;

		[Tooltip("How the plane should be drawn on top of the target plane.")]
		public PlaneDrawMode drawMode;

		[Tooltip("Should we use a custom material to draw this layer? Ignores the plane draw mode if set.")]
		public Material overrideMaterial = null;

		[Tooltip("When this flag is enabled, the render texture will not be cleared every frame. This only affects the top level rendering.")]
		public bool dontClear = false;

		internal CustomRenderTexture assignedRenderTexture;

	}
}
