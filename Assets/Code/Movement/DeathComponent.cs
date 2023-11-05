using Assets.Code.Helpers;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(TransformEventDispatcherComponent))]
[RequireComponent(typeof(SpriteRenderer))]
public class DeathComponent : MonoBehaviour, ITransientStart
{

	[Tooltip("The hitbox to query to see if we are hitting something that should kill us.")]
	public QueryableHitboxComponent deathHitbox;

	public GameObject deathParticlesPrefab;

	[Tooltip("The gravity component of the player, used to set velocity to 0")]
	public GravityComponent gravityComponent;

	public static event Action<Vector3> onGlobalDeath;

    /// <summary>
    /// The spawn point of the player
    /// </summary>
    public Vector3 currentSpawnPoint;
	private SpriteRenderer spriteRenderer;

	/// <summary>
	/// Invoked when the player dies.
	/// </summary>
	public event Action<Vector3> onDeath;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		currentSpawnPoint = transform.position;
		deathHitbox.onNewCollisionEnter += collidedThing =>
		{
			if (collidedThing.collider.gameObject.GetComponent<DeathProvider>() != null)
			{
				Kill();
			}
		};
	}

	/// <summary>
	/// Called from transient component via reflection
	/// </summary>
	public void TransientStart()
	{
		currentSpawnPoint = transform.position;
	}

	public void Kill()
	{
		try
		{
			onDeath?.Invoke(transform.position);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		try
		{
			onGlobalDeath?.Invoke(transform.position);
		}
		catch (Exception e)
		{
			Debug.LogError(e);
		}
		Debug.Log("<color='red'>The player has died, resetting their position!</color>");
		GetComponent<TransformEventDispatcherComponent>().DispatchTransformResetEvent();
		Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
		transform.position = new Vector3(currentSpawnPoint.x, currentSpawnPoint.y, transform.position.z);
		gravityComponent.velocity = 0;
		StartCoroutine(DeathAnimation());
	}

	public IEnumerator DeathAnimation()
	{
		FadeToBlack.FadeOut(0.15f, 0.3f);
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds(0.15f);
		spriteRenderer.enabled = true;
		GetComponent<TransformEventDispatcherComponent>().DispatchTransformResetEvent();
		transform.position = new Vector3(currentSpawnPoint.x, currentSpawnPoint.y, transform.position.z);
	}

}
