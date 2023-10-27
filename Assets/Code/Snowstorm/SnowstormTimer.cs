using Assets.Code.Snowstorm;
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
		while (blips.Length * (timeLeft / maxTime) < blipsShowing)
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
					blip.TemporarillyShowBlip();
				}
				i++;
			}
			blipsShowing--;
		}
	}

	/// <summary>
	/// Use this to show the blips to the player even if they aren't
	/// decreasing which can be used to show to the player that these
	/// are relevant to the storm.
	/// </summary>
	public void ShowBlips()
    {
		isTicking = true;
		foreach (SnowstormBlip blip in blips)
		{
			blip.TemporarillyShowBlip();
		}
    }

}
