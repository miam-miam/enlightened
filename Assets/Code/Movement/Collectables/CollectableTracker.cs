using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CollectableTracker : MonoBehaviour
{

	public Transform lightTransform;

	public static bool hasUnlockedPaintGun = false;

	private void Start()
	{
		// Shitcode
		SceneManager.activeSceneChanged += (oldscene, newscene) =>
		{
			if (newscene.name == "Introduction")
			{
				hasUnlockedPaintGun = false;
			}
		};
	}

	// Not enough time to do this properly
	private void Update()
	{
		
	}

}
