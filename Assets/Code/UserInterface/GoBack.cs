using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoBack : MonoBehaviour
{

    private void OnEnable()
    {
        StartCoroutine(SceneChangeAnimation("Introduction"));
    }

    public IEnumerator SceneChangeAnimation(string scene)
    {
        FadeToBlack.FadeOut(2, 2);
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(scene);
    }
}
