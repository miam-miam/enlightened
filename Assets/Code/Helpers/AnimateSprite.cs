using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimateSprite : MonoBehaviour
{

    [Tooltip("The frames of the animation")]
    public Sprite[] frames;

    [Tooltip("The time between each frame")]
    public float frameDelay;

    private SpriteRenderer[] spriteRenderers;

	private float nextFrame;

	private int currentFrame;

	// Start is called before the first frame update
	void Start()
    {
		spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
	}

	private void Update()
	{
		if (nextFrame > Time.time)
			return;
		nextFrame = Time.time + frameDelay;
		currentFrame = (currentFrame + 1) % frames.Length;
		foreach (var spriteRenderer in spriteRenderers)
			spriteRenderer.sprite = frames[currentFrame];
	}

}
