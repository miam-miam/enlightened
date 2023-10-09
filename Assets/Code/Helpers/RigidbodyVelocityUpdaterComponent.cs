using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyVelocityUpdaterComponent : MonoBehaviour
{

    private Vector2 previousPosition;

	private Rigidbody2D rigidBody;

	private void Start()
	{
		rigidBody = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame.
	void Update()
    {
		rigidBody.velocity = (rigidBody.position - previousPosition) / Time.deltaTime;
		previousPosition = rigidBody.position;
	}
}
