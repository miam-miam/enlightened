using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPaintable : Paintable
{
    public ObjectPool pool;

    private void Start()
    {
        if (pool == null)
        {
            pool = GetComponentInParent<ObjectPool>();
        }
    }

    public override void onPaint(QueryableHitboxComponent.ContactInformation info)
    {
        pool.PoolInstantiate(info.point, Quaternion.identity);
    }
}
