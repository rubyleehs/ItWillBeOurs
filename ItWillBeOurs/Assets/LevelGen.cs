using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorToPrefab
{
    public Color color;
    public GameObject prefab;
}

public class LevelGen : MonoBehaviour {

    public Texture2D map;
    public Transform mapParent;

    public ColorToPrefab[] colorMappings;
    public float tileSeperation;

    private void GenerateLevel()
    {
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                GenerateTile(x, y);
            }
        }
    }

    void GenerateTile(int x, int y)
    {
        Color _pixelColor = map.GetPixel(x, y);
        if (_pixelColor.a == 0) return;

        foreach (ColorToPrefab _colorMapping in colorMappings)
        {
            if (_colorMapping.color.Equals(_pixelColor))
            {
                Vector2 _pos = new Vector2(x, y) * tileSeperation;
                Instantiate(_colorMapping.prefab, _pos, Quaternion.identity, mapParent);
            }//
        }
    }
}
