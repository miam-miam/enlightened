using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Tileset", menuName = "Tileset", order = 1)]
public class Tileset : ScriptableObject
{

    public TilesetEntry[] tilesetEntries;

    public TilesetEntry Pick()
    {
        float tilset = tilesetEntries.Sum(x => x.weight);
        tilset = Random.Range(0, tilset);
		int i = 0;
        TilesetEntry tse = tilesetEntries[0];
		while (tilset > 0)
        {
			tse = tilesetEntries[i];
			tilset -= tse.weight;
			i++;
        }
        return tse;
	}

}
