using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.Level
{
	[RequireComponent(typeof(QueryableHitboxComponent))]
	internal class EndGame : MonoBehaviour
	{

		private void Start()
		{
			GetComponent<QueryableHitboxComponent>().onCollisionEnter += thing =>
			{
				StartCoroutine(EndTheGame(thing.collider.gameObject));
			};
		}

		public IEnumerator EndTheGame(GameObject thingToKill)
		{
			FadeToBlack.FadeOut(20, 5);
			yield return new WaitForSeconds(20);
			SceneManager.LoadScene("Credits");
			Transform t = thingToKill.transform;
			while (t.parent != null)
				t = t.parent;
			// Destroy the player once and for all
			Destroy(t.gameObject);
		}

	}
}
