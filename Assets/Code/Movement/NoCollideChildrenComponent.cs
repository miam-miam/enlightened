using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoCollideChildrenComponent : MonoBehaviour
{

    // Start is called before the first frame update.
    void Start()
    {
        var selfColliders = GetComponentsInChildren<Collider2D>();
        foreach (var col1 in selfColliders)
            foreach (var col2 in selfColliders)
                if (col1 != col2)
                    Physics2D.IgnoreCollision(col1, col2);
    }

}
