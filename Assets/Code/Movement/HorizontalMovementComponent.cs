using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovementComponent : MonoBehaviour
{

	private enum CollisionDirection
	{
		None = 0,
		Left = 1 << 0,
		Right = 1 << 1,
	}

    [Tooltip("The name of the keys to query for horizontal movement requests.")]
    public string horizontalMovementKeyAxis;

	[Tooltip("The rate at which we increase velocity while holding down a movement key.")]
	public float accelerationValue = 9f;

	[Tooltip("The hitbox that represents our center mass. When this collision box is entered, we will stop moving horizontally so long as the collided point was not above or below us.")]
	public QueryableHitboxComponent horizontalCollisionHitbox;

	[Tooltip("Our attached rigidbody.")]
	public Rigidbody2D playerRigidbody;

	[Tooltip("Deceleration due to friction.")]
	public float frictionAcceleration;

	private GravityComponent gravityComponent;

	/// <summary>
	/// How fast are we currently falling?
	/// </summary>
	public float HorizontalVelocity { get; set; } = 0;

	private CollisionDirection currentCollisionDirection = CollisionDirection.None;

	private void Start()
	{
		gravityComponent = GetComponent<GravityComponent>();
		horizontalCollisionHitbox.onCollisionEnter += collisionContact => {
			// If the collision point has the same horizontal level as us, then that means that we collided
			// into a wall, rather than into a floor / ceiling and in this case we should terminate our horizontal velocity.
			if (collisionContact.normal.x != 0)
			{
				// Left side colision
				if (collisionContact.normal.x > 0)
				{
					transform.position = new Vector3(collisionContact.point.x + (transform.localScale.x * 0.5f) + float.Epsilon, transform.position.y, transform.position.z);
					currentCollisionDirection |= CollisionDirection.Left;
					Debug.Log("<color='green'>[Horizontal]: Colliding with something to our left, pushing us to the right and disabling left movement.</color>");
				}
				// Right side collision
				else
				{
					transform.position = new Vector3(collisionContact.point.x - (transform.localScale.x * 0.5f) - float.Epsilon, transform.position.y, transform.position.z);
					currentCollisionDirection |= CollisionDirection.Right;
					Debug.Log("<color='green'>[Horizontal]: Colliding with something to our right, pushing us to the left and disabling right movement.</color>");
				}
				HorizontalVelocity = 0;
			}
			else
			{
				Debug.Log("<color='green'>[Horizontal]: Collision ignored due to not being on the horizontal axis.</color>");
			}
		};
		horizontalCollisionHitbox.onCollisionExit += collisionContact =>
		{
			currentCollisionDirection = CollisionDirection.None;
			Debug.Log("<color='green'>[Horizontal]: Horizontal collision movement freed.</color>");
		};
	}

	private void FixedUpdate()
	{
		if (SnowstormTimer.Instance.timeLeft < 0)
			return;
		float horizontalAxis = Input.GetAxis(horizontalMovementKeyAxis);
		float movementMultiplier = 1;
		if (Mathf.Sign(horizontalAxis) != Mathf.Sign(HorizontalVelocity))
		{
			movementMultiplier = Mathf.Max(Mathf.Abs(HorizontalVelocity), 1);
		}
		else
		{
			movementMultiplier = Mathf.Min(1, 7 / Mathf.Abs(movementMultiplier));
		}
		/**
		 * Horizontal movement acceleration
		 */
		HorizontalVelocity += Time.fixedDeltaTime * horizontalAxis * accelerationValue * movementMultiplier;
		/**
		 * Friction and air deceleration
		 */
		if (!gravityComponent.isFalling)
		{
			if (HorizontalVelocity < 0 && horizontalAxis >= 0)
			{
				// Horizontal velocity is negative
				// Add on either the friction acceleration or take us to 0.
				float decellerationSpeed = Mathf.Min(Mathf.Max(Mathf.Sqrt(-HorizontalVelocity), 1) * frictionAcceleration * Time.fixedDeltaTime, -HorizontalVelocity);
				HorizontalVelocity += decellerationSpeed;
			}
			else if (HorizontalVelocity > 0 && horizontalAxis <= 0)
			{
				float decellerationSpeed = Mathf.Min(Mathf.Max(Mathf.Sqrt(HorizontalVelocity), 1) * frictionAcceleration * Time.fixedDeltaTime, HorizontalVelocity);
				HorizontalVelocity -= decellerationSpeed;
			}
		}
		else
		{
			if (!gravityComponent.isFalling)
			{
				if (HorizontalVelocity < 0 && horizontalAxis > 0)
				{
					// Horizontal velocity is negative
					// Add on either the friction acceleration or take us to 0.
					float decellerationSpeed = Mathf.Min(Mathf.Max(Mathf.Sqrt(-HorizontalVelocity), 1) * frictionAcceleration * Time.fixedDeltaTime, -HorizontalVelocity);
					HorizontalVelocity += decellerationSpeed;
				}
				else if (HorizontalVelocity > 0 && horizontalAxis < 0)
				{
					float decellerationSpeed = Mathf.Min(Mathf.Max(Mathf.Sqrt(HorizontalVelocity), 1) * frictionAcceleration * Time.fixedDeltaTime, HorizontalVelocity);
					HorizontalVelocity -= decellerationSpeed;
				}
			}
		}
		if ((currentCollisionDirection & CollisionDirection.Left) != 0)
		{
			HorizontalVelocity = Mathf.Max(0, HorizontalVelocity);
		}
		if ((currentCollisionDirection & CollisionDirection.Right) != 0)
		{
			HorizontalVelocity = Mathf.Min(0, HorizontalVelocity);
		}
		// Limit the velocity to halff the size of the hitbox divided by delta time
		// If we are moving more than this speed, we can clip through objects
		// Note that this changes behaviour of the system based on framerate
		// This is super janky, but we have to do it
		float maxSpeed = (transform.localScale.x) / 3;
		transform.position += new Vector3(Mathf.Clamp(HorizontalVelocity * Time.fixedDeltaTime, -maxSpeed, maxSpeed), 0);
		Debug.DrawLine(transform.position, transform.position + new Vector3(-maxSpeed, 0, -1), horizontalCollisionHitbox.IsColliding ? Color.magenta : Color.black, Time.fixedDeltaTime, false);
		Debug.DrawLine(transform.position, transform.position + new Vector3(maxSpeed, 0, -1), horizontalCollisionHitbox.IsColliding ? Color.magenta : Color.black, Time.fixedDeltaTime, false);
	}

}
