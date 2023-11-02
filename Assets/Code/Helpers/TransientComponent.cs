using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TransientComponent : MonoBehaviour
{

    private static Dictionary<Type, MethodInfo> reflectionCache = new Dictionary<Type, MethodInfo>();

    private static Dictionary<string, GameObject> transientObjects = new Dictionary<string, GameObject>();

    [Tooltip("The transient key of this thing. If a transient copy of this item exists, this will be deleted on load and the existing object will be teleported to this location.")]
    public string transientKey;

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
                foreach (MonoBehaviour component in transientObject.GetComponentsInChildren<MonoBehaviour>())
                {
                    if (reflectionCache.TryGetValue(component.GetType(), out var method))
                    {
                        if (method != null)
                            method.Invoke(component, new object[0]);
                    }
                    else
                    {
                        MethodInfo locatedMethod = component.GetType().GetMethod("TransientStart", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (locatedMethod != null)
                            locatedMethod.Invoke(component, new object[0]);
                        reflectionCache.Add(component.GetType(), locatedMethod);
					}
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
