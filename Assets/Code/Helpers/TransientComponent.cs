using Assets.Code.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TransientComponent : MonoBehaviour, ITransientStart
{

    private static Dictionary<string, GameObject> transientObjects = new Dictionary<string, GameObject>();

    [Tooltip("The transient key of this thing. If a transient copy of this item exists, this will be deleted on load and the existing object will be teleported to this location.")]
    public string transientKey;

	public void TransientStart()
	{
		transform.position = Vector3.zero;
	}

	// Start is called before the first frame update
	void Start()
    {
        if (transientObjects.TryGetValue(transientKey, out var transientObject))
        {
            if (transientObject == null)
            {
                transientObjects[transientKey] = gameObject;
				DontDestroyOnLoad(gameObject);
			}
            else
            {
                transientObject.transform.position = new Vector3(transform.position.x, transform.position.y, transientObject.transform.position.z);
				// Invoke transient start on the transient object if the component has it.
				foreach (ITransientStart transient in transientObject.transform.GetComponentsInChildren<ITransientStart>(true))
				{
					transient.TransientStart();
				}
				Destroy(gameObject);
            }
        }
        else
        {
            transientObjects.Add(transientKey, gameObject);
            DontDestroyOnLoad(gameObject);
		}
    }

}
