using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashComponent : MonoBehaviour
{

    [Tooltip("Frequency of the flashes, 1 will be 1 second on and 1 second off.")]
    public float frequency;

    [Tooltip("Offset of the flash in seconds.")]
    public float offset;

    private SpriteRenderer spriteRenderer;

	private void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
    {
        spriteRenderer.enabled = (Time.time + offset) % (frequency * 2) >= frequency;
    }
}
