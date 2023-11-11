using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovingPlatform : MonoBehaviour
{
    
    [Tooltip("The hitbox that represents our bottom location. Once a collision on this is entered, then we will stop falling.")]
    public QueryableHitboxComponent gravityHitbox;

    public PaintSourceComponent paintSourceComponent;

    // Update is called once per frame
    void Update()
    {
        foreach (Collider2D collider in gravityHitbox.CollidingWith)
        {
			if (collider.GetComponentInParent<MoveAlongLine>() is MoveAlongLine platform)
			{
				var delta = platform.GetPositionDelta();
				transform.position += delta;
				paintSourceComponent.UpdateLastPositionBy(delta);
                break;
			}
        }
    }
}
