using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMovingPlatform : MonoBehaviour
{
    
    [Tooltip("The hitbox that represents our bottom location. Once a collision on this is entered, then we will stop falling.")]
    public QueryableHitboxComponent gravityHitbox;

    public PaintSourceComponent paintSourceComponent;

    private MoveAlongLine platform;
    // Start is called before the first frame update
    void Start()
    {
        gravityHitbox.onCollisionEnter += info =>
        {
            if (info.normal.y != 0)
            {
                MoveAlongLine plat = info.collider.GetComponentInParent<MoveAlongLine>();
                if (plat != null)
                {
                    platform = plat;
                }
            }
        };
        gravityHitbox.onCollisionExit += info =>
        {
            MoveAlongLine plat = info?.collider.GetComponentInParent<MoveAlongLine>();
            if (plat != null)
            {
                platform = null;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (platform != null)
        {
            var delta = platform.GetPositionDelta();
            transform.position += delta;
            paintSourceComponent.UpdateLastPositionBy(delta);
        }
    }
}
