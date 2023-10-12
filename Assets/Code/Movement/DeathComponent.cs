using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TransformEventDispatcherComponent))]
public class DeathComponent : MonoBehaviour
{

	[Tooltip("The hitbox to query to see if we are hitting something that should kill us.")]
	public QueryableHitboxComponent deathHitbox;

    /// <summary>
    /// The spawn point of the player
    /// </summary>
    public Vector3 currentSpawnPoint;

	/// <summary>
	/// Invoked when the player dies.
	/// </summary>
	public event Action<Vector3> onDeath;

	private void Start()
	{
		currentSpawnPoint = transform.position;
		deathHitbox.onNewCollisionEnter += collidedThing =>
		{
			if (collidedThing.collider.gameObject.GetComponent<DeathProvider>() != null)
			{
				Kill();
			}
		};
	}

	public void Kill()
	{
		onDeath?.Invoke(transform.position);
		Debug.Log("<color='red'>The player has died, resetting their position!</color>");
		GetComponent<TransformEventDispatcherComponent>().DispatchTransformResetEvent();
		transform.position = new Vector3(currentSpawnPoint.x, currentSpawnPoint.y, transform.position.z);
	}

}
