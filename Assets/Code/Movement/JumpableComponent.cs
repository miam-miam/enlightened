using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GravityComponent))]
public class JumpableComponent : MonoBehaviour
{

    [Tooltip("The input key that will be used to determine whether or not we are capable of jumping or not.")]
    public string jumpKey;

	[Tooltip("The hitbox that will be used to determine if we are allowed to jump or not.")]
	public QueryableHitboxComponent jumpHitbox;

	[Tooltip("The velocity at which we will jump.")]
	public float jumpVelocity = 10;

	[Tooltip("How many seconds after pressing the jump key should we count a jump if the user is in the air.")]
	public float jumpKeyStickiness = 0.2f;

	private float jumpKeyPressedAt = 0;

	private GravityComponent gravityComponent;

	/// <summary>
	/// Invoked when the player successfully jumps.
	/// </summary>
	public event Action onJumped;

	private void Start()
	{
		gravityComponent = GetComponent<GravityComponent>();
	}

	//test
	private void Update()
	{
		if (SnowstormTimer.Instance.timeLeft < 0)
			return;
		if (Input.GetButtonDown(jumpKey))
			jumpKeyPressedAt = Time.time;
		// Not attempting to jump
		if (jumpKeyPressedAt + jumpKeyStickiness < Time.time || !jumpHitbox.IsColliding)
			return;
		// Perform the jump
		gravityComponent.velocity = jumpVelocity;
		jumpKeyPressedAt = 0;
		onJumped?.Invoke();
	}

}
