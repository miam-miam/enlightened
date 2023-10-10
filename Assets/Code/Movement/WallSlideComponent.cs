using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
[RequireComponent(typeof(GravityComponent))]
[RequireComponent(typeof(JumpableComponent))]
[RequireComponent(typeof(HorizontalMovementComponent))]
public class WallSlideComponent : MonoBehaviour
{

    [Tooltip("The hitbox to use for determining if we are sliding on a wall to our right.")]
    public QueryableHitboxComponent rightHitbox;

    [Tooltip("The hitbox to use for determining if we are sliding on a wall to our left.")]
    public QueryableHitboxComponent leftHitbox;

    [Tooltip("What should our downwards acceleration be while sliding?")]
    public float slideAcceleration;

    [Tooltip("What should our maximum sliding speed be?")]
    public float slideSpeedMax;

    [Tooltip("The velocity to apply to the player in the horizontal direction when they do a walljump.")]
    public float wallJumpSpeed;

    [Tooltip("If you quickly change moving directions after walljumping, you will get a speed boost.")]
    public float wallJumpSpeedBoost;

    [Tooltip("Tiem in seconds after wall jumping that the boost should be given to us.")]
    public float wallJumpSpeedBoostTime;

    [Tooltip("The time in which after leaving the wall, we will still be able to walljump.")]
    public float wallJumpSafetyTime;

    // Track left and right collisions to add some deadzone for more lenient movement,
    private float leftCollisionTime;

    private float rightCollisionTime;

    // Track the jump time to give a speed impulse if we switch movement directions immediately after jumping
    private float jumpTime;

    private bool left;

    public bool IsSliding => rightHitbox.IsColliding || leftHitbox.IsColliding;

    private HorizontalMovementComponent horizontalMovement;

	private void Start()
	{
        GravityComponent gravityComponent = GetComponent<GravityComponent>();
		gravityComponent.interceptGravity += velocity =>
        {
            float moveDirection = Input.GetAxis("Horizontal");
            if (!(rightHitbox.IsColliding && moveDirection > 0) && !(leftHitbox.IsColliding && moveDirection < 0))
                return null;
            return Mathf.Max(velocity - slideAcceleration * Time.fixedDeltaTime, -slideSpeedMax);
        };
        horizontalMovement = GetComponent<HorizontalMovementComponent>();
		GetComponent<JumpableComponent>().onJumped += () =>
        {
            // We can only walljump if we are actually falling
            if (!gravityComponent.isFalling)
                return;
            if (rightCollisionTime + wallJumpSafetyTime > Time.fixedTime)
			{
				jumpTime = Time.fixedTime;
				horizontalMovement.HorizontalVelocity -= wallJumpSpeed;
                left = true;
			}
            else if (leftCollisionTime + wallJumpSafetyTime > Time.fixedTime)
			{
				jumpTime = Time.fixedTime;
				horizontalMovement.HorizontalVelocity += wallJumpSpeed;
                left = false;
			}
        };
	}

	private void FixedUpdate()
	{
		float moveDirection = Input.GetAxis("Horizontal");
		if (leftHitbox.IsColliding && moveDirection < 0)
            leftCollisionTime = Time.fixedTime;
        if (rightHitbox.IsColliding && moveDirection > 0)
            rightCollisionTime = Time.fixedTime;
        // If we recently did a wall jump, then let us get a speed boost when switching direction keys.
        if (jumpTime + wallJumpSpeedBoostTime > Time.fixedTime)
        {
            if (left && Input.GetAxis("Horizontal") < -0.2f)
            {
                jumpTime = 0;
                horizontalMovement.HorizontalVelocity -= wallJumpSpeedBoost;
			}
            else if (!left && Input.GetAxis("Horizontal") > 0.2f)
            {
                jumpTime = 0;
				horizontalMovement.HorizontalVelocity += wallJumpSpeedBoost;
			}
		}
	}

}
