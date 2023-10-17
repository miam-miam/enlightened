using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailsafeComponent : MonoBehaviour
{

    private Vector3 safeSpace;

    [Tooltip("The hitbox to use to see if we are actually inside of something at this point.")]
    public QueryableHitboxComponent collisionDetectionHitbox;

    [Tooltip("A tiny hitbox at the center of the player which determines if we ")]
    public QueryableHitboxComponent failsafeHitbox;

    // Start is called before the first frame update
    void Start()
    {
        safeSpace = transform.position;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!collisionDetectionHitbox.IsColliding)
        {
            safeSpace = transform.position;
		}
        if (failsafeHitbox.IsColliding)
		{
			GetComponent<TransformEventDispatcherComponent>().DispatchTransformResetEvent();
            Debug.LogWarning("Warning, failsafe event triggered!");
            Vector3 deltaMovement = safeSpace - transform.position;
			transform.position = safeSpace;
            GetComponent<HorizontalMovementComponent>().HorizontalVelocity = deltaMovement.x * Time.fixedDeltaTime;
            GetComponent<GravityComponent>().velocity = deltaMovement.y * Time.fixedDeltaTime;
		}
    }
}
