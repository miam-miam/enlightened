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

    public ParticleSystem rightWallSlideParticles;

    public ParticleSystem leftWallSlideParticles;

    // Same as left and right collision boxes, but requires the player to actively be trying to slide.
    private float leftSlideTime;

    private float rightSlideTime;
    
    private GravityComponent gravityComponent;

    // Track left and right collisions to add some deadzone for more lenient movement,
    private float leftCollisionTime;

    private float rightCollisionTime;

    // Track the jump time to give a speed impulse if we switch movement directions immediately after jumping
    private float jumpTime;

    private bool left;

    public bool IsSliding => rightHitbox.IsColliding || leftHitbox.IsColliding;

    private HorizontalMovementComponent horizontalMovement;

    private void SetWallSlideParticles(float velocity)
    {
	    if (velocity <= -slideSpeedMax)
	    {
		    if (rightHitbox.IsColliding)
		    {
			    if (!rightWallSlideParticles.isPlaying) rightWallSlideParticles.Play();
			    leftWallSlideParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		    }
		    else
		    {
			    if (!leftWallSlideParticles.isPlaying) leftWallSlideParticles.Play();
			    rightWallSlideParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		    }
	    }
    }

	private void Start()
	{
        gravityComponent = GetComponent<GravityComponent>();
        gravityComponent.interceptGravity += velocity =>
        {
            float moveDirection = Input.GetAxis("Horizontal");
            if (!(rightHitbox.IsColliding && moveDirection > 0) && !(leftHitbox.IsColliding && moveDirection < 0))
                return null;
            float newVelocity = velocity - slideAcceleration * Time.fixedDeltaTime;
            SetWallSlideParticles(newVelocity);
            return Mathf.Max(newVelocity, -slideSpeedMax);
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
                // Check if we can immediately perform a wall jump boost
                if (Input.GetAxis("Horizontal") < -0.2f)
				{
					jumpTime = 0;
					horizontalMovement.HorizontalVelocity -= wallJumpSpeedBoost;
				}
			}
            else if (leftCollisionTime + wallJumpSafetyTime > Time.fixedTime)
			{
				jumpTime = Time.fixedTime;
				horizontalMovement.HorizontalVelocity += wallJumpSpeed;
                left = false;
				// Check if we can immediately perform a wall jump boost
				if (Input.GetAxis("Horizontal") > 0.2f)
				{
					jumpTime = 0;
					horizontalMovement.HorizontalVelocity += wallJumpSpeedBoost;
				}
			}
        };
	}

	private void FixedUpdate()
	{
		float moveDirection = Input.GetAxis("Horizontal");
        if (leftHitbox.IsColliding)
        {
            if (moveDirection < 0)
                leftSlideTime = Time.fixedTime;
            leftCollisionTime = Time.fixedTime;
        }
        if (rightHitbox.IsColliding)
        {
            if (moveDirection > 0)
                rightSlideTime = Time.fixedTime;
            rightCollisionTime = Time.fixedTime;
        }
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

        if (gravityComponent.velocity > -slideSpeedMax || 
            !gravityComponent.isFalling || 
            !(rightHitbox.IsColliding && moveDirection > 0) && !(leftHitbox.IsColliding && moveDirection < 0))
        {
	        leftWallSlideParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	        rightWallSlideParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
	}

}
