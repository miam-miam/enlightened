using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSourceComponent : MonoBehaviour
{

	[Tooltip("The object that represents the paint that will be sprayed on static objects.")]
	public GameObject staticPaint;

    [Tooltip("The hitbox that should be used to determine whether or not we should paint a surface. Anything colliding with this hitbox will be painted.")]
    public QueryableHitboxComponent paintHitbox;

	private void Start()
	{
		var staticLevel = new ContactFilter2D();
		staticLevel.SetLayerMask(LayerMask.NameToLayer("StaticLevel"));
		
		var dynamicLevel = new ContactFilter2D();
		dynamicLevel.SetLayerMask(LayerMask.NameToLayer("DynamicLevel"));
		
		paintHitbox.onNewCollisionEnter += collisionContact =>
		{
			if (collisionContact.collider.IsTouching(staticLevel))
			{
				staticPaint.SetActive(true);
			}
		};
		
		paintHitbox.onCollisionEnter += collisionContact =>
		{
			if (collisionContact.collider.IsTouching(staticLevel))
			{
				staticPaint.SetActive(true);
			}
		};

		
		paintHitbox.onCollisionExit += _ =>
		{
			staticPaint.SetActive(false);
		};
	}

}
