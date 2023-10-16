using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformEventDispatcherComponent : MonoBehaviour
{

    public event Action dispatchTransformResetEvent;

    // Start is called before the first frame update
    void Start()
    {
        foreach (QueryableHitboxComponent hitboxComponent in GetComponentsInChildren<QueryableHitboxComponent>())
        {
            dispatchTransformResetEvent += () =>
            {
                hitboxComponent.ResetHitboxDispatcher();
            };

		}
	}

    /// <summary>
    /// Inform our hitboxes that a transform reset event occurred
    /// </summary>
    public void DispatchTransformResetEvent()
    {
        dispatchTransformResetEvent?.Invoke();
	}

}
