using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyeComponent : MonoBehaviour
{

	public enum EyeSide
	{
		LEFT,
		RIGHT
	}

	[Tooltip("The maximum distance that the eyes are allowed to move.")]
	public float maxEyeDistance = 1f;

	[Tooltip("How far should the eyes be from the mouse horizontally.")]
	public float eyeSeperation = 0.233f;

	[Tooltip("The side of the face that the eye is on")]
	public EyeSide eyeSide = EyeSide.LEFT;

	private Vector3 lastPosition;

	private void Update()
	{
		Vector3 deltaPosition = transform.position - lastPosition;
		lastPosition = transform.position;
		Vector3 desired = deltaPosition * maxEyeDistance;
		desired.z = transform.localPosition.z;
		desired.x = Mathf.Clamp(desired.x, -maxEyeDistance, maxEyeDistance) + ((eyeSide == EyeSide.RIGHT ? 1 : -1) * eyeSeperation);
		desired.y = Mathf.Clamp(desired.y, -maxEyeDistance, maxEyeDistance);
		transform.localPosition = desired;
	}
}
