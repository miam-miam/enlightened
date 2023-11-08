using System.Collections;
using System.Collections.Generic;
using Assets.Code.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LevelTransitionerComponent : MonoBehaviour, ITransientStart
{ 
    public string entryPoint = "";

    [Tooltip("The game object that holds the static paint.")]
    public GameObject staticPaint;

    public void TransientStart(bool calledAgain)
    {
        if (!calledAgain && entryPoint != "")
        {
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PlayerSpawners"))
            {
                var component = obj.GetComponent<PlayerSpawner>();
                if (component != null)
                {
                    component.Spawn(entryPoint);
                }
            }
        }
    }

    public void ResetStaticPaint()
    {
        staticPaint.SetActive(false);
        StartCoroutine(ReactivateStaticPaint());
    }

    private IEnumerator ReactivateStaticPaint()
    {
        yield return 0;
        staticPaint.SetActive(true);   
    }
}
