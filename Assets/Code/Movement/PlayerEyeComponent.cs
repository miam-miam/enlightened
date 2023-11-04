using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerEyeComponent;

public class PlayerEyeComponent : MonoBehaviour
{

	public enum EyeSide
	{
		LEFT,
		RIGHT
	}

	public enum FollowTarget
	{
		Movement,
		Mouse
	}

	[Tooltip("The maximum distance that the eyes are allowed to move.")]
	public float maxEyeDistance = 1f;

	[Tooltip("How far should the eyes be from the mouse horizontally.")]
	public float eyeSeperation = 0.233f;

	[Tooltip("The side of the face that the eye is on")]
	public EyeSide eyeSide = EyeSide.LEFT;

	[Tooltip("What should the eyes be following?")]
	public FollowTarget followTarget = FollowTarget.Movement;

	private Vector3 lastPosition;

	private void Update()
	{
		Vector3 deltaPosition;
		Vector3 desired;
		switch (followTarget)
		{
			case FollowTarget.Movement:
				deltaPosition = transform.position - lastPosition;
				lastPosition = transform.position;
				desired = deltaPosition * maxEyeDistance;
				desired.z = transform.localPosition.z;
				desired.x = Mathf.Clamp(desired.x, -maxEyeDistance, maxEyeDistance) + ((eyeSide == EyeSide.RIGHT ? 1 : -1) * eyeSeperation);
				desired.y = Mathf.Clamp(desired.y, -maxEyeDistance, maxEyeDistance);
				transform.localPosition = desired;
				break;
			case FollowTarget.Mouse:
				deltaPosition = (Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f)) / Mathf.Min(Screen.width * 0.5f, Screen.height * 0.5f);
				lastPosition = transform.position;
				desired = deltaPosition * maxEyeDistance;
				desired.z = transform.localPosition.z;
				desired.x = Mathf.Clamp(desired.x, -maxEyeDistance, maxEyeDistance) + ((eyeSide == EyeSide.RIGHT ? 1 : -1) * eyeSeperation);
				desired.y = Mathf.Clamp(desired.y, -maxEyeDistance, maxEyeDistance);
				transform.localPosition = desired;
				break;
		}
	}
}
