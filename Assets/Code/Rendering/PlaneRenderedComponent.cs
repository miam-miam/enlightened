using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneRenderedComponent : MonoBehaviour
{

    [Tooltip("The plane that we are rendering to.")]
    public PlaneMaster plane;

	// Start is called before the first frame update
	internal void Start()
    {
        if (PlaneRenderingSourceComponent.staged != null)
        {
            PlaneRenderingSourceComponent.staged.Add(this);
			return;
        }
        // Everything must be sorted by sorting groups and not z.
        transform.position = new Vector3(transform.position.x, transform.position.y, plane.AssignedZ);
    }

}
