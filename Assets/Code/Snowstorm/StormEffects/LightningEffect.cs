using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(AudioSource))]
public class LightningEffect : SnowstormEffectComponent
{

    private SpriteRenderer spriteRenderer;

    private AudioSource audioSource;

    private bool lightningPlaying = false;

	// Start is called before the first frame update
	void Start()
    {
        audioSource = GetComponent<AudioSource>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		onTick += intensity =>
        {
            if (Random.Range(0f, 30f) < intensity)
            {
                StartCoroutine(FlashLightning(intensity));
            }
        };
    }

    private IEnumerator FlashLightning(float intensity)
    {
        if (lightningPlaying)
            yield break;
        lightningPlaying = true;
        audioSource.Play();
        // As the storm gets stronger, the lightning comes closer
        yield return new WaitForSeconds(1 + 3 * (1 - intensity));
		int i = -2;
        while (Random.Range(0, 10) > (i += 2))
        {
            spriteRenderer.color = WithAlpha(1);
			yield return new WaitForFixedUpdate();
			spriteRenderer.color = WithAlpha(0);
			yield return new WaitForSeconds(Random.Range(0f, 0.05f));
		}
		float alpha = 1;
        while (alpha > 0)
		{
			spriteRenderer.color = WithAlpha(alpha);
			yield return new WaitForEndOfFrame();
            alpha -= Time.fixedDeltaTime * 0.5f;
		}
		spriteRenderer.color = WithAlpha(0);
        while (audioSource.isPlaying)
            yield return new WaitForFixedUpdate();
		lightningPlaying = false;
	}

    private Color WithAlpha(float alpha)
    {
        return new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
    }

}
