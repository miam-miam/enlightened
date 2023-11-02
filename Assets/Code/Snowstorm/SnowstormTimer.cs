using Assets.Code.Snowstorm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowstormTimer : MonoBehaviour
{

	public static bool isTicking = false;

	[Tooltip("The blips for the snowstorm timers.")]
	public SnowstormBlip[] blips;

	[Tooltip("How much time did the player have when they started to reach the next checkpofint?")]
	public float maxTime = 300;

	[Tooltip("How much time do we have left until the storm arrives?")]
	public float timeLeft = 300;

	private int blipsShowing;

	private void Start()
	{
		blipsShowing = blips.Length - 1;
	}

	private void Update()
	{
		if (!isTicking)
			return;
		timeLeft -= Time.deltaTime;
		int blipsWanted = (int)Math.Ceiling(blips.Length * (timeLeft / maxTime));
		if (blipsWanted == 1)
		{
			// Start flashing the blip
			if (blipsShowing > 1)
			{
				for (int i = blips.Length - 1; i >= 1; i--)
					blips[i].EndBlip();
				blips[0].ShowBadness();
				blipsShowing = 1;
			}
		}
		else
		{
			while (blipsWanted < blipsShowing)
			{
				int i = 1;
				foreach (SnowstormBlip blip in blips)
				{
					if (i > blipsShowing)
					{
						blip.EndBlip();
					}
					else
					{
						blip.TemporarillyShowBlip(5);
					}
					i++;
				}
				blipsShowing--;
			}
		}
	}

	/// <summary>
	/// Use this to show the blips to the player even if they aren't
	/// decreasing which can be used to show to the player that these
	/// are relevant to the storm.
	/// </summary>
	public void ShowBlips(float time)
    {
		isTicking = true;
		foreach (SnowstormBlip blip in blips)
		{
			blip.TemporarillyShowBlip(time);
		}
    }

}
