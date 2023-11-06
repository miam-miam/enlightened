using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSourceComponent : MonoBehaviour
{

	[Tooltip("The hitbox that should be used to determine whether or not we should paint a surface. Anything colliding with this hitbox will be painted.")]
    public QueryableHitboxComponent paintHitbox;

    [Tooltip("How far in square magnitude should the player go to paint again.")]
    public float paintDistance;
    
    private Paintable paintable;
    private Vector3 lastPaintPosition;
    private Vector2 positionOffset;
    

	private void Start()
	{
		paintHitbox.onCollisionEnter += collisionContact =>
		{
			var obj = collisionContact.collider.GetComponent<Paintable>();
			if (obj != null)
			{
				obj.onPaint(collisionContact);
				paintable = obj;
				lastPaintPosition = transform.position;
				positionOffset = collisionContact.point - (Vector2) transform.position;
			}
		};
	}

	private void Update()
	{
		var dist = (lastPaintPosition - transform.position).sqrMagnitude;
		if (dist > paintDistance && paintable != null)
		{
			lastPaintPosition = transform.position;
			paintable.onPaint(new QueryableHitboxComponent.ContactInformation(null, positionOffset + (Vector2) lastPaintPosition, new Vector2()));
		}
	}
}
