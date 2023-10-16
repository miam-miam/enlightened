using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintParticleComponent : MonoBehaviour
{
    
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents = new();
    public GameObject paintPrefab;
    public PlaneMaster particlePlane;

    private int dynamicLevelLayer;

    private void Start()
    {
        dynamicLevelLayer = LayerMask.NameToLayer("DynamicLevel");
    }

    private void OnParticleCollision(GameObject other)
    {
        collisionEvents.Clear();
        var length = part.GetCollisionEvents(other, collisionEvents);
        if (length == 0)
        {
            return;
        }
        
        if (other.layer == dynamicLevelLayer)
        {
            var paintable = other.GetComponent<Paintable>();
            if (paintable != null)
            {
                paintable.onPaint(new QueryableHitboxComponent.ContactInformation(
                    (Collider2D)collisionEvents[0].colliderComponent, 
                    collisionEvents[0].intersection, 
                    collisionEvents[0].normal));
            }
        }
        else
        {
            var point = collisionEvents[0].intersection;
            point.z = particlePlane.AssignedZ;
            Instantiate(paintPrefab, point, Quaternion.identity);
        } 
    }
}
