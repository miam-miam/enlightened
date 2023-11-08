using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Code.Helpers;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Tooltip("The entry point for which this spawner should respond to.")]
    public string entryPoint;
    
    
    public void Spawn(string requestedEntryPoint)
    {
        if (requestedEntryPoint == entryPoint)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = new Vector3(transform.position.x, transform.position.y, player.transform.position.z);
            foreach (ITransientStart transient in player.GetComponentsInChildren<ITransientStart>(true))
            {
                try
                {
                    transient.TransientStart(true);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
    }
}
