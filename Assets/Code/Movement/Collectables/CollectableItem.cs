using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(QueryableHitboxComponent))]
public class CollectableItem : MonoBehaviour
{

	private bool pickedUp = false;

	public event Action<CollectableTracker> applyEffect;

	// Start is called before the first frame update
	void Start()
    {
		GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += collisionDetails =>
		{
			if (pickedUp)
				return;
			CollectableTracker tracker;
			if ((tracker = collisionDetails.collider.GetComponentInParent<CollectableTracker>()) == null)
				return;
			pickedUp = true;
			StartCoroutine(PickupAnimation());
			applyEffect?.Invoke(tracker);
		};
		SetEffect();
	}

	public virtual void SetEffect()
	{ }

	public IEnumerator PickupAnimation()
	{
		SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
		float currentScale = 1;
		while (currentScale < 3)
		{
			currentScale += Time.deltaTime * 6;
			transform.localScale = new Vector3(currentScale, currentScale, 1);
			foreach (var spriteRenderer in spriteRenderers)
				spriteRenderer.color = new Color(1, 1, 1, 1 - ((currentScale - 1) / 2));
			yield return new WaitForEndOfFrame();
		}
	}

}
