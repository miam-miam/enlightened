using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Paintable : MonoBehaviour
{
    // Will be null in the case of particles.
    public abstract void onPaint(QueryableHitboxComponent.ContactInformation info);
}
