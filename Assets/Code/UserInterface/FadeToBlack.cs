using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeToBlack : MonoBehaviour
{

	private static FadeToBlack singleton;

	/// <summary>
	/// The image that we will be black.
	/// </summary>
	private Image image;

	private float currentFadeAmount;
	private float fadeStartAmount;

	public float fadeStartTime = 0;
	public float fadeMiddleTime = 0;
	public float fadeEndTime = 0;

	private void Start()
	{
		singleton = this;
		image = GetComponent<Image>();
	}

	private void Update()
	{
		if (Time.time < fadeMiddleTime)
		{
			currentFadeAmount = Mathf.Lerp(fadeStartAmount, 1, (Time.time - fadeStartTime) / (fadeMiddleTime - fadeStartTime));
		}
		else if (Time.time < fadeEndTime)
		{
			currentFadeAmount = Mathf.Lerp(1, 0, (Time.time - fadeMiddleTime) / (fadeEndTime - fadeMiddleTime));
		}
		else
			return;
		currentFadeAmount = Mathf.Clamp01(currentFadeAmount);
		image.color = new Color(0, 0, 0, currentFadeAmount);
	}

	public static void FadeOut(float inTime, float outTime)
	{
		singleton.fadeStartAmount = singleton.currentFadeAmount;
		singleton.fadeStartTime = Time.time;
		singleton.fadeMiddleTime = Mathf.Max(Time.time + inTime, singleton.fadeMiddleTime);
		singleton.fadeEndTime = Mathf.Max(Time.time + outTime, singleton.fadeEndTime);
	}

}
