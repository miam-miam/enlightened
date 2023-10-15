using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSourceComponent : MonoBehaviour
{

	[Tooltip("The hitbox that should be used to determine whether or not we should paint a surface. Anything colliding with this hitbox will be painted.")]
    public QueryableHitboxComponent paintHitbox;

	private void Start()
	{
		paintHitbox.onNewCollisionEnter += collisionContact =>
		{
			var paintable = collisionContact.collider.GetComponent<Paintable>();
			if (paintable != null)
			{
				paintable.onPaint(collisionContact);
			}
		};
	}

}
