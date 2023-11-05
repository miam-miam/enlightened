using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Code.UserInterface
{
	internal class ButtonActions : MonoBehaviour
	{

		public void QuitGame()
		{
			StartCoroutine(QuitAnimation());
		}

		public void ChangeScene(string scene)
		{
			StartCoroutine(SceneChangeAnimation(scene));
		}

		public IEnumerator SceneChangeAnimation(string scene)
		{
			FadeToBlack.FadeOut(2, 2);
			yield return new WaitForSeconds(2);
			SceneManager.LoadScene(scene);
		}

		public IEnumerator QuitAnimation()
		{
			FadeToBlack.FadeOut(2, 2);
			yield return new WaitForSeconds(2);
			Application.Quit();
		}

	}
}
