using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipOnSecondView : MonoBehaviour
{
    public Animator[] animators;

    private static bool firstTime = true;
    // Start is called before the first frame update
    void Start()
    {
        if (firstTime)
        {
            firstTime = false;
        }
        else
        {
            foreach (var anim in animators)
            {
                anim.speed = Mathf.Infinity;
            }
        }
    }

}
