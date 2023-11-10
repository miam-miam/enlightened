using Assets.Code.Snowstorm;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(QueryableHitboxComponent))]
public class CampfireZone : MonoBehaviour
{

	public TextMeshProUGUI[] hintText;

	public static bool blockingPlayerMovement = false;

	private bool triggered = false;

	private void Start()
	{
		GetComponent<QueryableHitboxComponent>().onNewCollisionEnter += details =>
		{
			if (triggered)
				return;
			// Don't trigger if we already saved here, you need to find a different checkpoint fool!
			if (SnowstormTimer.LastMajorCheckpoint == SceneManager.GetActiveScene().name)
				return;
			if (details.collider.tag != "Player")
				return;
			triggered = true;
			StartCoroutine(SleepAnimation());
		};
	}

	public IEnumerator SleepAnimation()
    {
		blockingPlayerMovement = true;
		SnowstormTimer.isTicking = false;
		SnowstormTimer.LastMajorCheckpoint = SceneManager.GetActiveScene().name;
		while (PlaneRenderingSourceComponent.planeMasterController == null)
			yield return new WaitForEndOfFrame();
		float currentFade = 1;
		while (currentFade > 0)
		{
			currentFade -= Time.deltaTime * 0.2f;
			foreach (var pm in PlaneRenderingSourceComponent.planeMasterController.planes)
			{
				if (!pm.drawToScreen)
					continue;
				// Fade the alpha of the colour plane away
				Matrix4x4 mat = pm.ColourMatrix;
				mat[3, 3] = currentFade;
				pm.ColourMatrix = mat;
			}
			yield return new WaitForEndOfFrame();
		}
		float fontFadeIn = 0;
		while (fontFadeIn < 1)
		{
			fontFadeIn += Time.deltaTime;
			foreach (var ht in hintText)
				ht.color = new Color(ht.color.r, ht.color.g, ht.color.b, fontFadeIn);
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(2);
		// Restore the blips
		foreach (var blip in SnowstormTimer.Instance.blips)
		{
			blip.HideInstantly();
		}
		int i = 0;
		foreach (var blip in SnowstormTimer.Instance.blips)
		{
			blip.TerminateBadness();
			blip.TemporarillyShowBlip(SnowstormTimer.Instance.blips.Length - i + 3);
			yield return new WaitForSeconds(1);
			i++;
		}
		yield return new WaitForSeconds(3);
		SnowstormTimer.Instance.timeLeft = SnowstormTimer.Instance.maxTime;
		blockingPlayerMovement = false;
		while (currentFade < 1)
		{
			currentFade = Mathf.Min(currentFade + Time.deltaTime * 0.5f, 1);
			foreach (var pm in PlaneRenderingSourceComponent.planeMasterController.planes)
			{
				if (!pm.drawToScreen)
					continue;
				// Fade the alpha of the colour plane away
				Matrix4x4 mat = pm.ColourMatrix;
				mat[3, 3] = currentFade;
				pm.ColourMatrix = mat;
			}
			foreach (var ht in hintText)
				ht.color = new Color(ht.color.r, ht.color.g, ht.color.b, 1 - currentFade);
			yield return new WaitForEndOfFrame();
		}
		SnowstormTimer.isTicking = true;
	}

}
