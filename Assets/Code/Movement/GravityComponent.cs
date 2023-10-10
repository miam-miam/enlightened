using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityComponent : MonoBehaviour
{

	[Tooltip("Acceleration due to gravity.")]
	public float gravitationalConstant = 9f;

    [Tooltip("The hitbox that represents our bottom location. Once a collision on this is entered, then we will stop falling.")]
    public QueryableHitboxComponent gravityHitbox;

	[Tooltip("The hitbox of the head of the player.")]
	public QueryableHitboxComponent headHitbox;

	[Tooltip("Our attached rigidbody.")]
	public Rigidbody2D playerRigidbody;

	[Tooltip("The position of the very bottom of this object, used to determine where to snap to on landing.")]
	public GameObject floorPoint;

	/// <summary>
	/// Are we currently falling?
	/// </summary>
	[SerializeField]
	internal bool isFalling = false;

	[SerializeField]
	private bool blockedAbove = false;

	/// <summary>
	/// Intercept the gravity function in order to provide custom behaviours instead.
	/// Only gets called when we are falling, if we are moving upwards then this call will be skipped.
	/// </summary>
	public event Func<float, float?> interceptGravity;

	/// <summary>
	/// How fast are we currently falling?
	/// </summary>
	public float velocity { get; set; } = 0;

	private void Start()
	{
		isFalling = !gravityHitbox.IsColliding;
		gravityHitbox.onCollisionEnter += collisionContact => {
			if (collisionContact.normal.y != 0)
			{
				isFalling = false;
				// Teleport to the point of collision to prevent entering the floor
				// This needs to be a vertical collision, if we embedded in the wall slightly due to moving fast
				// horizontally, then we don't want to teleport upwards since that would just make wall jumping
				// impossible.
				if (collisionContact.normal.y > 0)
				{
					Debug.Log("<color='blue'>[Gravity]: Collusion point was below the transform, pushing us up.</color>");
					transform.position = new Vector2(transform.position.x, collisionContact.point.y - floorPoint.transform.localPosition.y);
				}
				else
				{
					Debug.Log("<color='blue'>[Gravity]: Collision point above the transform, disabling gravity.</color>");
				}
			}
			else
			{
				Debug.Log("<color='blue'>[Gravity]: Collision ignored due to not being on the vertical axis.</color>");
			}
		};
		gravityHitbox.onCollisionExit += collisionContact => {
			isFalling = true;
			velocity = Mathf.Max(0, velocity);
			Debug.Log("<color='blue'>[Gravity]: Collision ended.</color>");
		};
		headHitbox.onCollisionEnter += collisionContact => {
			if (collisionContact.normal.y != 0)
			{
				if (collisionContact.normal.y < 0)
				{
					velocity = Mathf.Min(0, velocity);
					blockedAbove = true;
					transform.position = new Vector2(transform.position.x, collisionContact.point.y - transform.lossyScale.y * 0.5f);
				}
			}
		};
		headHitbox.onCollisionExit += collisionContact => {
			blockedAbove = false;
		};
	}

	private void FixedUpdate()
	{
		float maxSpeed = (transform.lossyScale.y - 0.03f) / 2;
		Debug.DrawLine(transform.position, transform.position + new Vector3(0, -maxSpeed, -1), Color.black, Time.fixedDeltaTime, false);
		Debug.DrawLine(transform.position, transform.position + new Vector3(0, maxSpeed, -1), Color.white, Time.fixedDeltaTime, false);
		// Allow processing of vertical velocity if our vertical velocity is positive
		if (!isFalling && velocity <= 0)
			return;
		float? interceptedResult;
		if (velocity <= 0 && (interceptedResult = interceptGravity?.Invoke(velocity)) != null)
		{
			velocity = interceptedResult.Value;
			// Can't be moving up if we are blocked above
			if (blockedAbove)
				velocity = Mathf.Min(0, velocity);
		}
		else
		{
			if (blockedAbove)
				velocity = Mathf.Min(0, velocity);
			velocity -= gravitationalConstant * Time.fixedDeltaTime;
		}
		transform.position += new Vector3(0, Mathf.Clamp(velocity * Time.fixedDeltaTime, -maxSpeed, maxSpeed));
	}

}
