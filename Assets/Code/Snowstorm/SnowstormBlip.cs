using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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

		public void TemporarillyShowBlip()
		{
			if (isHidden)
				return;
			StartCoroutine(TemporarillyShowBlipAnimation());
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

		private IEnumerator TemporarillyShowBlipAnimation()
		{
			if (isHidden)
				yield break;
			float time = 5;
			while (time > 0)
			{
				time -= Time.deltaTime;
				float value = Mathf.Clamp01(5 - time) * Mathf.Clamp01(time);
				foreach (SpriteRenderer image in images)
				{
					image.color = new Color(image.color.r, image.color.b, image.color.g, value);
				}
				yield return new WaitForEndOfFrame();
				if (isHidden)
					yield break;
			}
		}

	}
}
