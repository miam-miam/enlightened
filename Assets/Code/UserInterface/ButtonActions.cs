using System;
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
			Application.Quit();
		}

		public void ChangeScene(string scene)
		{
			SceneManager.LoadScene(scene);
		}

	}
}
