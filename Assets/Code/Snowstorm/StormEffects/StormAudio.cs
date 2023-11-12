using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class StormAudio : SnowstormEffectComponent
{

	private void Start()
	{
		AudioSource audioSource = GetComponent<AudioSource>();
		float maxVolume = audioSource.volume;
		audioSource.volume = 0;
		onTick += (power, real) =>
		{
			if (!real)
				return;
			audioSource.volume = power * maxVolume;
		};
		onEnd += () =>
		{
			audioSource.volume = 0;
		};
	}

}
