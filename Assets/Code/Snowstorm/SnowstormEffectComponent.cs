using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An effect who's appearance depends on the time of the snowstorm.
/// </summary>
public abstract class SnowstormEffectComponent : MonoBehaviour
{

	[Tooltip("The time left in the cycle that the effect will appear at.")]
	public float startTime;

	[Tooltip("The time left in the cycle where the effect will fade out. Should be lower than the start time.")]
	public float endTime;

	[Tooltip("How long should the effect take to fade in?")]
	public float fadeInTime;

	[Tooltip("How long should the effect take to fade out?")]
	public float fadeOutTime;

	/// <summary>
	/// Invoke when the effect begins.
	/// </summary>
	public event Action onStart;

	/// <summary>
	/// Invoked every frame that the effect is active for.
	/// The value passed is a value between 0 and 1 representing the fade time.
	/// </summary>
	public event Action<float> onTick;

	/// <summary>
	/// Invoked when the time expires and the effect is no longer playing.
	/// </summary>
	public event Action onEnd;

	private bool started = false;

	private void Update()
	{
		if (SnowstormTimer.Instance.timeLeft <= startTime)
		{
			if (SnowstormTimer.Instance.timeLeft > endTime)
			{
				if (!started)
				{
					onStart?.Invoke();
					started = true;
				}
				onTick?.Invoke(Mathf.Clamp01((startTime - SnowstormTimer.Instance.timeLeft) / Mathf.Max(fadeInTime, 1)) * Mathf.Clamp01((SnowstormTimer.Instance.timeLeft - endTime) / Mathf.Max(fadeInTime, 1)));
			}
			else if (started)
			{
				started = false;
				onEnd?.Invoke();
			}
		}
	}

}
