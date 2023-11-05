using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowstormShake : SnowstormEffectComponent
{

    [Tooltip("Should we shake the children instead of ourselves?")]
    public bool shakeChildren = false;

    [Tooltip("How strong should the shake effect be??")]
    public float strength = 1;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 initialPosition = transform.position;
        onTick += (intensity, real) =>
        {
            if (!shakeChildren)
            {
                Vector3 desiredPosition = initialPosition + new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0) * strength;
                transform.position = new Vector3(desiredPosition.x, desiredPosition.y, transform.position.z);
            }
            else
			{
				Vector3 desiredPosition = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0) * strength;
				foreach (Transform child in GetComponentsInChildren<Transform>())
                {
                    if (child == transform)
                        continue;
                    child.localPosition = new Vector3(desiredPosition.x, desiredPosition.y, child.localPosition.z);
				}
            }
		};
    }
}
