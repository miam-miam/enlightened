using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class FollowMovingPlatform : MonoBehaviour
{
    
    [Tooltip("The hitbox that represents our bottom location. Once a collision on this is entered, then we will stop falling.")]
    public QueryableHitboxComponent gravityHitbox;

    public PaintSourceComponent paintSourceComponent;

    // Update is called once per frame
    void Update()
    {
		Vector3? requestedDelta = null;
		foreach (Collider2D collider in gravityHitbox.CollidingWith)
        {
			if (collider.GetComponentInParent<MoveAlongLine>() is MoveAlongLine platform)
			{
				requestedDelta = platform.GetPositionDelta();
				break;
			}
		}
		if (requestedDelta == null)
			return;
		if (requestedDelta.Value.y < 0)
		{
			foreach (Collider2D collider in gravityHitbox.CollidingWith)
			{
				// If we are colliding with the ground, ignore the moving platform
				if (collider.GetComponentInParent<MoveAlongLine>() == null)
				{
					return;
				}
			}
		}
		transform.position += requestedDelta.Value;
		paintSourceComponent.UpdateLastPositionBy(requestedDelta.Value);
	}
}
