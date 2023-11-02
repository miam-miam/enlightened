using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code.Snowstorm
{
	public class SnowstormBlip : MonoBehaviour
	{

		[Tooltip("The images used for the snowstorm blips.")]
		public SpriteRenderer[] images;

		private bool isHidden = false;

		private void Start()
		{
			foreach (SpriteRenderer image in images)
			{
				image.color = new Color(image.color.r, image.color.b, image.color.g, 0);
			}
		}

		public void ShowBadness()
		{
			if (isHidden)
				return;
			StartCoroutine(AhShit());
		}

		public void TemporarillyShowBlip(float time)
		{
			if (isHidden)
				return;
			StartCoroutine(TemporarillyShowBlipAnimation(time));
		}

		public void EndBlip()
		{
			if (isHidden)
				return;
			StartCoroutine(BlipOutAnimation());
		}

		private IEnumerator BlipOutAnimation()
		{
			isHidden = true;
			float initialScale = images[0].transform.localScale.x;
			float time = 2;
			while (time > 0)
			{
				time -= Time.deltaTime;
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(image.color.r, image.color.b, image.color.g, Mathf.Clamp01(time) * Mathf.Clamp01(2 - time));
					image.transform.localScale = new Vector3((2 - Mathf.Clamp01(time)) * initialScale, (2 - Mathf.Clamp01(time)) * initialScale, 1);
				}
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator TemporarillyShowBlipAnimation(float timeFor)
		{
			if (isHidden)
				yield break;
			float time = timeFor;
			while (time > 0)
			{
				time -= Time.deltaTime;
				float value = Mathf.Clamp01(timeFor - time) * Mathf.Clamp01(time);
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(image.color.r, image.color.b, image.color.g, value);
				}
				yield return new WaitForEndOfFrame();
				if (isHidden)
					yield break;
			}
		}

		private IEnumerator AhShit()
		{
			if (isHidden)
				yield break;
			float time = 1;
			while (time > 0)
			{
				time -= Time.deltaTime;
				float value = Mathf.Clamp01(time);
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(image.color.r, image.color.b, image.color.g, 1 - value);
				}
				yield return new WaitForEndOfFrame();
				if (isHidden)
					yield break;
			}
			while (!isHidden)
			{
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(1, 1, 1, 1);
				}
				yield return new WaitForSeconds(0.5f);
				if (isHidden)
					yield break;
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(1, 0, 0, 1);
				}
				yield return new WaitForSeconds(0.5f);
			}
		}

	}
}
