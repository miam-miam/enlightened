using Assets.Code.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QueryableHitboxComponent))]
public class PaletteChangeTrigger : MonoBehaviour
{

    public Texture newPalette;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += collisionDetails =>
        {
            if (collisionDetails.collider.GetComponentInParent<PaletteChangeReciever>() == null)
                return;
            foreach (CustomRenderTexture crt in PlaneMaster.activeRenderTextures)
            {
                crt.material?.SetTexture("_LevelPalette", newPalette);
				crt.initializationMaterial?.SetTexture("_LevelPalette", newPalette);
			}
        };
    }
}