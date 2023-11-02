using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DeathParticle : SnowstormEffectComponent
{
    // Start is called before the first frame update
    void Start()
    {
		onStart += () =>
		{
			GetComponent<ParticleSystem>().Play();
		};
	}
}
