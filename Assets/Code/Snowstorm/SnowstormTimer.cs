using Assets.Code.Snowstorm;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnowstormTimer : MonoBehaviour
{

	public static string LastMajorCheckpoint = "tutorial_00";

	public static SnowstormTimer Instance;

	public static bool isTicking = false;

	public static int timesDied = 0;

	[Tooltip("The blips for the snowstorm timers.")]
	public SnowstormBlip[] blips;

	[Tooltip("How much time did the player have when they started to reach the next checkpofint?")]
	public float maxTime = 300;

	[Tooltip("How much time do we have left until the storm arrives?")]
	public float timeLeft = 300;

	/// <summary>
	/// Called when the time left until the storm is updated.
	/// </summary>
	public event Action<float> onTimeLeftUpdated;

	private int blipsShowing;

	private void Start()
	{
		blipsShowing = blips.Length - 1;
		if (Instance == null)
			Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance == this)
			Instance = null;
	}

	private void Update()
	{
		if (!isTicking)
			return;
		if (timeLeft < -2 && Input.GetButtonDown("Jump"))
		{
			// Restart the game.
			StartCoroutine(RestartGame());
		}
		timeLeft -= Time.deltaTime;
		int blipsWanted = (int)Math.Ceiling(blips.Length * (timeLeft / maxTime));
		if (blipsWanted > blipsShowing)
		{
			foreach (SnowstormBlip blip in blips)
			{
				blip.TerminateBadness();
				blip.TemporarillyShowBlip(5);
			}
			blipsShowing = blips.Length;
		}
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
				int i = 2;
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
		onTimeLeftUpdated?.Invoke(timeLeft);
	}

	private IEnumerator RestartGame()
	{
		FadeToBlack.FadeOut(2, 2);
		yield return new WaitForSeconds(2);
		SceneManager.LoadScene(LastMajorCheckpoint);
		timesDied++;
		// Reset the storm timer
		maxTime = 180 + 43 * Mathf.Log(timesDied + 1);
		timeLeft = maxTime;
		onTimeLeftUpdated?.Invoke(timeLeft);
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
