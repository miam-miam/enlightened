using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(QueryableHitboxComponent))]
public class CollectableItem : MonoBehaviour
{

    [Tooltip("The text hints attached to this item.")]
    public TextMeshProUGUI[] textMeshes;

    private bool fadeingIn = false;

	private Action<QueryableHitboxComponent.ContactInformation> fadeIn;

	private Action<QueryableHitboxComponent.ContactInformation> fadeOut;

	// Start is called before the first frame update
	void Start()
    {
        foreach (var thing in textMeshes)
            thing.color = new Color(thing.color.r, thing.color.g, thing.color.b, 0);
		fadeIn = collisionDetails =>
		{
			CollectableTracker tracker;
			if ((tracker = collisionDetails.collider.GetComponentInParent<CollectableTracker>()) == null)
				return;
			fadeingIn = true;
		};
		GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += fadeIn;
		fadeOut = collisionDetails =>
		{
			CollectableTracker tracker;
			if ((tracker = collisionDetails.collider.GetComponentInParent<CollectableTracker>()) == null)
				return;
			fadeingIn = false;
		};
		GetComponent<QueryableHitboxComponent>().onCollisionExit += fadeOut;
	}

	private void OnDestroy()
	{
		GetComponent<QueryableHitboxComponent>().onNewCollisionEnter -= fadeIn;
		GetComponent<QueryableHitboxComponent>().onCollisionExit -= fadeOut;
	}

	private void Update()
	{
		foreach (var thing in textMeshes)
			thing.color = new Color(thing.color.r, thing.color.g, thing.color.b, Mathf.Clamp01(thing.color.a + Time.deltaTime * (fadeingIn ? 1 : -1)));
	}

}
