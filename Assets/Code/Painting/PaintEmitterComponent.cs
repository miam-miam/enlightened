using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PaintEmitterComponent : MonoBehaviour
{

    public ParticleSystem leftPaintEmitter;
    public ParticleSystem rightPaintEmitter;
    public QueryableHitboxComponent JumpHitbox;
    public GameObject playerGameObject; 
    public float emitHorizontalSpeed;
    private float previousXPosition;

    private void Start()
    {
        previousXPosition = playerGameObject.transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var speed = (playerGameObject.transform.position.x - previousXPosition) / Time.fixedDeltaTime;
        if (JumpHitbox.IsColliding && speed >= emitHorizontalSpeed)
        {
            if (!leftPaintEmitter.isPlaying) leftPaintEmitter.Play();
            rightPaintEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else if (JumpHitbox.IsColliding && speed <= -emitHorizontalSpeed)
        {
            if (!rightPaintEmitter.isPlaying) rightPaintEmitter.Play();
            leftPaintEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        else
        {
            leftPaintEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            rightPaintEmitter.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
        previousXPosition = playerGameObject.transform.position.x;
    }
}
