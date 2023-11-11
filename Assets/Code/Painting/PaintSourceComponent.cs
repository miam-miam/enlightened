using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSourceComponent : MonoBehaviour
{

	[Tooltip("The hitbox that should be used to determine whether or not we should paint a surface. Anything colliding with this hitbox will be painted.")]
    public QueryableHitboxComponent paintHitbox;

    [Tooltip("Offset the paint spawning by this amount.")]
    public Vector2 paintOffset = new(0, 0.25f);
    
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
				collisionContact.point += paintOffset;
				obj.onPaint(collisionContact);
				paintable = obj;
				lastPaintPosition = transform.position;
				positionOffset = collisionContact.point - (Vector2) transform.position;
			}
		};

		paintHitbox.onCollisionExit += collisionContact =>
		{
			if (collisionContact?.collider != null && collisionContact.collider.GetComponent<Paintable>() != null)
			{
				paintable = null;
			}
		};
	}

	private void FixedUpdate()
	{
		var dist = (lastPaintPosition - transform.position).sqrMagnitude;
		if (dist > paintDistance && paintable != null)
		{
			lastPaintPosition = transform.position;
			paintable.onPaint(new QueryableHitboxComponent.ContactInformation(null, positionOffset + (Vector2) lastPaintPosition, new Vector2()));
		}
	}

	public void UpdateLastPositionBy(Vector3 offset)
	{
		lastPaintPosition += offset;
	}
}
