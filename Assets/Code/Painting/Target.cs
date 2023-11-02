using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : Paintable
{
    public event Action onActivate;
    public GameObject paintSprite;
    public SpriteRenderer light;
    public Color activatedColour;

    public override void onPaint(QueryableHitboxComponent.ContactInformation info)
    {
        onActivate?.Invoke();
        paintSprite.SetActive(true);
        light.color = activatedColour;
    }
}
