using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(QueryableHitboxComponent))]
public class LevelTransition : MonoBehaviour
{

    private QueryableHitboxComponent queryableHitboxComponent;

	[Tooltip("The name of the level to change to when we hit this trigger.")]
	public string newLevel;

	private static Dictionary<string, AsyncOperation> loadedLevels = new();

	[Tooltip("The name of the next entrypoint from which the player should be spawned in from.")]
	public string newEntryPoint;

	private string currentLevelName;

	private AsyncOperation dontGcMe;

	// Start is called before the first frame update
	void Start()
	{
		queryableHitboxComponent = GetComponent<QueryableHitboxComponent>();
		if (FindObjectsOfType<LevelTransition>().Length > 1)
		{
			queryableHitboxComponent.onNewCollisionEnter += collision =>
			{
				var component = collision.collider.gameObject.GetComponent<LevelTransitionerComponent>();
				if (component == null)
					return;
				SceneManager.LoadScene(newLevel);
				loadedLevels.Clear();
				component.entryPoint = newEntryPoint;
				component.ResetStaticPaint();
			};
			return;
		}
		AsyncOperation asyncOperation;
		if (!loadedLevels.ContainsKey(newLevel))
		{
			asyncOperation = SceneManager.LoadSceneAsync(newLevel);
			asyncOperation.allowSceneActivation = false;
			loadedLevels.Add(newLevel, asyncOperation);
		}
		else
		{
			asyncOperation = loadedLevels[newLevel];
		}
		
		StartCoroutine(ActivateCollisions(asyncOperation));
	}

	public IEnumerator ActivateCollisions(AsyncOperation asyncOperation)
	{
		// Wait a fixed update to ensure that the player entered the trigger and didn't just spawn there.
		yield return new WaitForFixedUpdate();
		queryableHitboxComponent.onNewCollisionEnter += collision =>
		{
			var component = collision.collider.gameObject.GetComponent<LevelTransitionerComponent>();
			if (component == null)
				return;
			foreach (var otherLoad in loadedLevels)
				otherLoad.Value.priority = -10000;
			asyncOperation.allowSceneActivation = true;
			asyncOperation.priority = 100;
			dontGcMe = asyncOperation;
			loadedLevels.Clear();
			component.entryPoint = newEntryPoint;
			component.ResetStaticPaint();
		};
	}
}
