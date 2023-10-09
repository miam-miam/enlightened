using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintSourceComponent : MonoBehaviour
{

	[Tooltip("The object that represents the paint that will be sprayed on objects.")]
	public GameObject paintPrefab;

    [Tooltip("The hitbox that should be used to determine whether or not we should paint a surface. Anything colliding with this hitbox will be painted.")]
    public QueryableHitboxComponent paintHitbox;

	private void Start()
	{
		paintHitbox.onCollisionEnter += collisionPosition =>
		{
			Instantiate(paintPrefab, collisionPosition, Quaternion.identity);
		};
	}

}
