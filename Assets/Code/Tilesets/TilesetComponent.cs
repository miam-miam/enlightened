using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilesetComponent : MonoBehaviour
{

    public Tileset tileset;

    // Start is called before the first frame update
    void Start()
    {
        Sprite newSprite = tileset.Pick().sprite;
        if (newSprite == null)
            return;
		GetComponent<SpriteRenderer>().sprite = newSprite;
    }

}
