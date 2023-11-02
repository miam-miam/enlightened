using Assets.Code.Helpers;
using JetBrains.Annotations;
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
public class QueryableHitboxComponent : MonoBehaviour, ITransientStart
{

	public enum CollisionMode
	{
		COLLISION_TRIGGERS = 1 << 0,
		COLLISION_PHYSICAL = 1 << 1,
		COLLISION_ALL = COLLISION_TRIGGERS | COLLISION_PHYSICAL,
	}

	public class ContactInformation
	{
		public Collider2D collider;
		public Vector2 point;
		public Vector2 normal;

		public ContactInformation(Collider2D collider, Vector2 point, Vector2 normal)
		{
			this.collider = collider;
			this.point = point;
			this.normal = normal;
		}

	}

	private int _colCount = 0;

	public bool IsColliding {
		// Support for quick query
		get => isColliding || collidedThisFrame;
		private set => isColliding = value;
	}
	/// <summary>
	/// Called when this collider begins colliding with something.
	/// Parameter passed is the position of the collision point closest to our transform.position point.
	/// </summary>
	public event Action<ContactInformation> onCollisionEnter;

	/// <summary>
	/// Called when the collider begins colliding with a new hitbox.
	/// </summary>
	public event Action<ContactInformation> onNewCollisionEnter;

#nullable enable
	/// <summary>
	/// Called when this collider ends colliding with something.
	/// </summary>
	public event Action<ContactInformation?>? onCollisionExit;
#nullable restore

	private bool collidedThisFrame = false;
	private int colliderCount = 0;

	// A really stupid way of doing dynamic arrays
	private List<ContactInformation> collidersTouchedThisFrame = new List<ContactInformation>(16);

	private ContactInformation[] debugContactInfo;

	private Collider2D selfCollider;

	private Vector2 colPoint;

	public bool skipFrame = false;

	public bool ignoreTriggers = false;

	[Tooltip("What colour do we want this hitbox to be?")]
	public Color debugColour = Color.red;
	private bool isColliding = false;

	[Tooltip("How should this hitbox respond to the other collider type?")]
	public CollisionMode collisionDetectionMode = CollisionMode.COLLISION_ALL;

	private void Start()
	{
		selfCollider = GetComponent<Collider2D>();
	}

	/// <summary>
	/// Called from transient component via reflection.
	/// Level changed, so we need to clear all our collisions.
	/// </summary>
	public void TransientStart()
	{
		ResetHitboxDispatcher();
		transform.parent.position = Vector3.zero;
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
		if (skipFrame)
		{
			skipFrame = false;
			return;
		}
		if (colliderCount == 0)
			return;
		// Find the best collider
		ContactInformation bestContact = collidersTouchedThisFrame[0];
		float bestDistance = Vector2.Distance(transform.position, bestContact.point);
		bool isDef = true;
		for (int i = 1; i < colliderCount; i++)
		{
			ContactInformation nextContact = collidersTouchedThisFrame[i];
			float nextDistance = Vector2.Distance(transform.position, nextContact.point);
			if (nextDistance >= bestDistance || !selfCollider.IsTouching(collidersTouchedThisFrame[i].collider))
				continue;
			bestDistance = nextDistance;
			bestContact = nextContact;
			isDef = false;
		}
		//Debug.Log($"<color='orange'>{gameObject.name}: Finding best collision out of {colliderCount} hitboxes.</color>");
		// Refresh
		// Note that this maintains memory references, but it won't leak since it will override them when needed.
		collidedThisFrame = false;
		colliderCount = 0;
		// If we stopped colliding with everything we were colliding with, then don't invoke it
		if (isDef && !selfCollider.IsTouching(collidersTouchedThisFrame[0].collider))
			return;
		debugContactInfo = collidersTouchedThisFrame.Take(colliderCount).ToArray();
		colPoint = bestContact.point;
		// Collide with the thing that we touched the closest
		onCollisionEnter?.Invoke(bestContact);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_TRIGGERS) == 0) || (!collision.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_PHYSICAL) == 0))
			return;
		onNewCollisionEnter?.Invoke(CalculateContactInformation(collision));
		// Check if we are still colliding
		if (!collision.IsTouching(selfCollider))
			return;
		_colCount++;
		if (!isColliding)
			collidedThisFrame = true;
		if (collidedThisFrame)
			collidersTouchedThisFrame.ExpandingAdd(colliderCount++, CalculateContactInformation(collision));
		isColliding = _colCount > 0;
		//Debug.Log($"<color='orange'>{gameObject.name}: Collision entered with {collision.name}</color>");
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((collision.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_TRIGGERS) == 0) || (!collision.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_PHYSICAL) == 0))
			return;
		_colCount--;
		isColliding = _colCount > 0;
		// If we collided this frame, then we haven't actually invoked collision enter yet so
		// we can just wait on that.
		if (!isColliding && !collidedThisFrame)
		{
			onCollisionExit?.Invoke(CalculateContactInformation(collision));
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if ((collision.collider.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_TRIGGERS) == 0) || (!collision.collider.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_PHYSICAL) == 0))
			return;
		onNewCollisionEnter?.Invoke(CalculateContactInformation(collision.collider));
		// Check if we are still colliding
		if (!collision.collider.IsTouching(selfCollider))
			return;
		_colCount++;
		if (!isColliding)
			collidedThisFrame = true;
		if (collidedThisFrame)
		{
			collidersTouchedThisFrame.ExpandingAdd(colliderCount++, CalculateContactInformation(collision.collider));
		}
		isColliding = _colCount > 0;
		//Debug.Log($"<color='orange'>{gameObject.name}: Collision entered with {collision.collider.name}</color>");
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if ((collision.collider.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_TRIGGERS) == 0) || (!collision.collider.isTrigger && (collisionDetectionMode & CollisionMode.COLLISION_PHYSICAL) == 0))
			return;
		_colCount--;
		isColliding = _colCount > 0;
		// If we collided this frame, then we haven't actually invoked collision enter yet so
		// we can just wait on that.
		if (!isColliding && !collidedThisFrame)
		{
			onCollisionExit?.Invoke(CalculateContactInformation(collision.collider));
		}
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		// Draw debug information
		_ = CalculateContactInformation(collision.collider);
	}

	private ContactInformation CalculateContactInformation(Collider2D collider)
	{
		if (!collider.gameObject.activeInHierarchy)
			return null;
		var info = CalculateContactInformation(collider, transform.position);
		Debug.DrawLine(info.point, info.point + info.normal, debugColour);
		return info;
	}
	
	public static ContactInformation CalculateContactInformation(Collider2D collider, Vector2 position)
	{
		Vector2 colPoint = collider.ClosestPoint(position);
		PhysicsShapeGroup2D result = new PhysicsShapeGroup2D();
		collider.GetShapes(result);
		// We don't support circles
		if (result.vertexCount == 0)
			throw new NotImplementedException("Calculate contact information doesn't support circle colliders.");
		List<Vector2> vertices = new List<Vector2>();
		Vector2 closestPoint = Vector2.zero;
		Vector2 closestNormal = Vector2.down;
		float closestDistance = Mathf.Infinity;
		// Find the closest point on the hitbox to our current player position, and
		// calculate the normal which goes closest towards the center of the player hitbox.
		for (int i = 0; i < result.shapeCount; i++)
		{
			vertices.Clear();
			result.GetShapeVertices(i, vertices);
			for (int j = 0; j < vertices.Count; j++)
			{
				Vector2 p1 = vertices[j];
				Vector2 p2 = vertices[(j + 1) % vertices.Count];
				Vector2 closestPointOnLine = (position).ClosestPointOnLine(p1, p2);
				float distanceFromSource = Vector2.Distance(closestPointOnLine, position);
				// If the point is further away than our current point ignore.
				if (closestDistance < distanceFromSource)
					continue;
				// If 2 points are equally far apart from the player, resolve the dispute by picking
				// the one with the normal that points more towards the player's center than the other.
				// This will prevent corner collisions from picking the wrong normal side since on a
				// top left corner, both up and left are valid normals since the closest point is a vertex.
				Vector2 currentNormal = (p2 - p1).CalculatePerpendicularLine(position);
				if (closestDistance == distanceFromSource)
				{
					float bestAngle = Vector2.Angle(closestNormal, position - closestPoint);
					float newAngle = Vector2.Angle(currentNormal, position - closestPoint);
					//Debug.Log($"Best:{bestAngle} vs New:{newAngle}");
					if (bestAngle < newAngle)
						continue;
				}
				closestDistance = distanceFromSource;
				closestPoint = closestPointOnLine;
				closestNormal = currentNormal;
			}
		}
		// Determine the normal
		return new ContactInformation(collider, closestPoint, closestNormal);
	}

	/// <summary>
	/// Reset the hitbox event dispatcher manager.
	/// This informs the hitbox manager to skip the next frame, so that unity can run
	/// the next fixed update and recalculate the state of the colliders. If we don't do this
	/// then we will trip hitboxes that we have already moved away from.
	/// </summary>
	public void ResetHitboxDispatcher()
	{
		// If we dispatched a collision enter event, send the associated collision exit event.
		if (_colCount > 0 && !collidedThisFrame)
		{
			onCollisionExit?.Invoke(null);
		}
		collidedThisFrame = false;
		colliderCount = 0;
		skipFrame = true;
		//Debug.Log($"<color='orange'>{gameObject.name}: Player was teleported, clearing queued hitboxes</color>");
	}

}
