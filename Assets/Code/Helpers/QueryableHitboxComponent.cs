using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A hitbox that can be queried by other components in order to determine
/// if we are colliding or have entered a collision.
/// This will handle cases where you start colliding with multiple colliders at once,
/// since we are using tile based levels.
/// 
/// Trigger hitboxes have less accurate point detection, so if you want to prevent
/// objects from moving through other objects you should use a non-trigger hitbox.
/// This is because the point of contact is given by non-trigger hitboxes but has
/// to be determined by trigger hitboxes.
/// 
/// Trigger hitboxes should be used mainly for seeing if we are colliding, for
/// example being used to determine if we can jump or not.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class QueryableHitboxComponent : MonoBehaviour
{

	private class ContactInformation
	{
		public Collider2D collider;
		public Vector2 point;
		public ContactInformation(Collider2D collider, Vector2 point)
		{
			this.collider = collider;
			this.point = point;
		}
	}

	private int _colCount = 0;

    public bool IsColliding { get; private set; } = false;

	/// <summary>
	/// Called when this collider begins colliding with something.
	/// Parameter passed is the position of the collision point closest to our transform.position point.
	/// </summary>
	public event Action<Vector2> onCollisionEnter;

	/// <summary>
	/// Called when this collider ends colliding with something.
	/// </summary>
	public event Action<Vector2> onCollisionExit;

	private bool collidedThisFrame = false;
	private int colliderCount = 0;
	private List<ContactInformation> collidersTouchedThisFrame = new List<ContactInformation>(16);

	private ContactInformation[] debugContactInfo;

	private Collider2D selfCollider;

	private Vector2 colPoint;

	[Tooltip("What colour do we want this hitbox to be?")]
	public Color debugColour = Color.red;

	private void Start()
	{
		selfCollider = GetComponent<Collider2D>();
	}

	/// <summary>
	/// Every physics step, we need to find everything that we started colliding with in the previous frame
	/// and calculate which one of those colliders was the closest collider to us.
	/// 
	/// Should this be a fixed update?
	/// It would make more sense, but then we can see ourselves visually going into the floor since rendering happens in Update() and not
	/// in FixedUpdate()
	/// 
	/// Most of this is pointless, since I coded it for the gravity component, however then I realised I only
	/// need to bother with the Y position and not update the X position in there, but I'm keeping this around
	/// since we will probably have a case with wall jumping where we need to use it for the X position but not the Y position.
	/// </summary>
	void FixedUpdate()
	{
		if (IsColliding && !collidedThisFrame)
		{
			Debug.DrawLine(transform.position, colPoint, debugColour);
			if (debugContactInfo != null)
			{
				foreach (ContactInformation ci in debugContactInfo)
				{
					if (ci == null)
						continue;
					Debug.DrawLine(transform.position, ci.point, debugColour);
				}
			}
		}
		if (colliderCount == 0)
			return;
		// Find the best collider
		Vector2 bestPosition = collidersTouchedThisFrame[0].collider.ClosestPoint(transform.position);
		float bestDistance = Vector2.Distance(transform.position, bestPosition);
		bool isDef = true;
		for (int i = 1; i < colliderCount; i++)
		{
			Vector2 nextPosition = collidersTouchedThisFrame[i].collider.ClosestPoint(transform.position);
			float nextDistance = Vector2.Distance(transform.position, nextPosition);
			if (nextDistance >= bestDistance || !selfCollider.IsTouching(collidersTouchedThisFrame[i].collider))
				continue;
			bestDistance = nextDistance;
			bestPosition = nextPosition;
			isDef = false;
		}
		// Refresh
		// Note that this maintains memory references, but it won't leak since it will override them when needed.
		collidedThisFrame = false;
		colliderCount = 0;
		// If we stopped colliding with everything we were colliding with, then don't invoke it
		if (isDef && !selfCollider.IsTouching(collidersTouchedThisFrame[0].collider))
			return;
		debugContactInfo = collidersTouchedThisFrame.Take(colliderCount).ToArray();
		colPoint = bestPosition;
		// Collide with the thing that we touched the closest
		onCollisionEnter?.Invoke(bestPosition);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		_colCount++;
		if (!IsColliding)
			collidedThisFrame = true;
		if (collidedThisFrame)
			collidersTouchedThisFrame.ExpandingAdd(colliderCount++, new ContactInformation(collision, collision.ClosestPoint(transform.position)));
		IsColliding = _colCount > 0;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		_colCount--;
		IsColliding = _colCount > 0;
		// If we collided this frame, then we haven't actually invoked collision enter yet so
		// we can just wait on that.
		if (!IsColliding && !collidedThisFrame)
		{
			onCollisionExit?.Invoke(collision.ClosestPoint(transform.position));
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		_colCount++;
		if (!IsColliding)
			collidedThisFrame = true;
		if (collidedThisFrame)
		{
			collidersTouchedThisFrame.ExpandingAdd(colliderCount++, new ContactInformation(collision.collider, collision.collider.ClosestPoint(transform.position)));
		}
		IsColliding = _colCount > 0;
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		_colCount--;
		IsColliding = _colCount > 0;
		// If we collided this frame, then we haven't actually invoked collision enter yet so
		// we can just wait on that.
		if (!IsColliding && !collidedThisFrame)
		{
			onCollisionExit?.Invoke(collision.collider.ClosestPoint(transform.position));
		}
	}

}
