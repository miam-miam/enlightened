using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SnowstormDeath : SnowstormEffectComponent
{
    // Start is called before the first frame update
    void Start()
    {
        onStart += () =>
        {
            StartCoroutine(Die());
        };
    }

    private IEnumerator Die()
    {
        GetComponent<SpriteRenderer>().material = new Material(GetComponent<SpriteRenderer>().material);
		GetComponent<SpriteRenderer>().material.SetFloat("_StartTime", Time.time);
        yield return new WaitForSeconds(2);
        // 10 hours should be long enough to read that. Anything too high gets floating point inaccuracy
        FindObjectOfType<CinematicText>().DisplayText("Game Over", "Press space to restart", 60 * 60 * 10);
    }
}
