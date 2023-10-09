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

	/// <summary>
	/// How fast are we currently falling?
	/// </summary>
	public float HorizontalVelocity { get; set; } = 0;

	private CollisionDirection currentCollisionDirection = CollisionDirection.None;

	private void Start()
	{
		horizontalCollisionHitbox.onCollisionEnter += closestPoint => {
			// If the collision point has the same horizontal level as us, then that means that we collided
			// into a wall, rather than into a floor / ceiling and in this case we should terminate our horizontal velocity.
			if (closestPoint.y <= transform.position.y + 0.03f && closestPoint.y >= transform.position.y - 0.03f)
			{
				// Left side colision
				if (closestPoint.x < transform.position.x)
				{
					transform.position = new Vector2(closestPoint.x + (transform.localScale.x * 0.5f) + float.Epsilon, transform.position.y);
					currentCollisionDirection |= CollisionDirection.Left;
					Debug.Log("<color='green'>[Horizontal]: Colliding with something to our left, pushing us to the right and disabling left movement.</color>");
				}
				// Right side collision
				else
				{
					transform.position = new Vector2(closestPoint.x - (transform.localScale.x * 0.5f) - float.Epsilon, transform.position.y);
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
		horizontalCollisionHitbox.onCollisionExit += closestPoint =>
		{
			currentCollisionDirection = CollisionDirection.None;
			Debug.Log("<color='green'>[Horizontal]: Horizontal collision movement freed.</color>");
		};
	}

	private void FixedUpdate()
	{
		HorizontalVelocity += Time.fixedDeltaTime * Input.GetAxis(horizontalMovementKeyAxis) * accelerationValue;
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
