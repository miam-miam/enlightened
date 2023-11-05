using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class StormSpriteRenderer : SnowstormEffectComponent
{

	[Tooltip("The properties to inject the intensity into of shaders.")]
	public string[] shaderIntensityProperties = new string[0];

	[Tooltip("Should this effect play if the effect is fake?")]
	public bool renderIfFake = true;

	// Start is called before the first frame update
	void Start()
    {
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
		onTick += (power, real) =>
        {
			if (!renderIfFake && !real)
				return;
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, power);
			foreach (string shaderProp in shaderIntensityProperties)
				spriteRenderer.material.SetFloat(shaderProp, power);
		};
    }
}
