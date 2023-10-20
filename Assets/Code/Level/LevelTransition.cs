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

	// Start is called before the first frame update
	void Start()
    {
		queryableHitboxComponent = GetComponent<QueryableHitboxComponent>();
		queryableHitboxComponent.onNewCollisionEnter += collision => {
			if (collision.collider.gameObject.GetComponent<LevelTransitionerComponent>() == null)
				return;
			SceneManager.LoadScene(newLevel);
		};
	}
}
