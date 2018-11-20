using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ColorMapping
{
    public Color color;
    public int prefabID;
    public GameObject prefab;
}

public class VisualGen : MathfExtras {

    public Transform mapParent;

    public ColorMapping[] colorMappings;
    public Color nullColor;
    public float tileSeperation;

    protected void SetAreaTexValue(Texture2D _tex, ref int[,] _intMap, Vector2Int _fromIndex, bool flipX, bool flipY, bool clockRot90)
    {
        int h = _tex.height;
        int w = _tex.width;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int texX = IndexFlipper(x, w -1, flipX);
                int texY = IndexFlipper(y, h -1, flipY);
                ColorMapping _cMap = GetTexColorMapping(_tex, texX, texY);

                Vector2Int intMapChangeIndexLot;
                if (!clockRot90) intMapChangeIndexLot = new Vector2Int(_fromIndex.x + x, _fromIndex.y + y);
                else intMapChangeIndexLot = new Vector2Int(y + _fromIndex.x,h - x - 1 +_fromIndex.y);
                // 0,0 > 0,h
                // w,0 > 0,0
                // w,h > w,0
                // 0,h > w,h

                if (_cMap != null) _intMap[intMapChangeIndexLot.x,intMapChangeIndexLot.y] = _cMap.prefabID;
                else _intMap[intMapChangeIndexLot.x, intMapChangeIndexLot.y] = 0;
            }
        
        }
    }
    protected void SetAreaTexValue(Texture2D _tex, ref int[,] _intMap, Vector2Int _fromIndex, int _fromValue, bool flipX, bool flipY, bool clockRot90)
    {
        int h = _tex.height;
        int w = _tex.width;
        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                int texX = IndexFlipper(x, w - 1, flipX);
                int texY = IndexFlipper(y, h - 1, flipY);
                ColorMapping _cMap = GetTexColorMapping(_tex, texX, texY);

                Vector2Int intMapChangeIndexLot;
                if (!clockRot90) intMapChangeIndexLot = new Vector2Int(_fromIndex.x + x, _fromIndex.y + y);
                else intMapChangeIndexLot = new Vector2Int(y + _fromIndex.x, h - x - 1 + _fromIndex.y);
                // 0,0 > 0,h
                // w,0 > 0,0
                // w,h > w,0
                // 0,h > w,h

                if (_intMap[intMapChangeIndexLot.x, intMapChangeIndexLot.y] == _fromValue)
                {
                   if (_cMap != null) _intMap[intMapChangeIndexLot.x, intMapChangeIndexLot.y] = _cMap.prefabID;
                   else _intMap[intMapChangeIndexLot.x, intMapChangeIndexLot.y] = 0;
                }
            }

        }
    }

    int IndexFlipper(int index, int maxIndex, bool flip)
    {
        if (flip) return maxIndex - index;
        else return index;
    }


    /*
    public void GenerateTexVisuals(Texture2D _tex, int _xIndexOffset, int _yIndexOffset, Vector2Int _pivot, int _quadrant)
    {
        int h = _tex.height;
        int w = _tex.width;
        int _xRotIndexOffset = _xIndexOffset;
        int _yRotIndexOffset = _yIndexOffset;
        

        switch (_quadrant)
        { 
            case 1:
                _xRotIndexOffset = _pivot.x - (_xIndexOffset - _pivot.x);
                _yRotIndexOffset = _pivot.y - (_yIndexOffset - _pivot.y);

                for (int dy = 0; dy < h; dy++)
                {
                    for (int dx = 0; dx < w; dx++)
                    {
                        GameObject _texTile = GetTexTile(_tex,h- dx -1, w-dy -1);

                        if (_texTile != null) Instantiate(_texTile, new Vector2(dx + _xRotIndexOffset, dy + _yRotIndexOffset) * tileSeperation, Quaternion.identity, mapParent);
                    }
                }
                break;
            case 2:
                _xRotIndexOffset = _pivot.x + (_yIndexOffset - _pivot.y);
                _yRotIndexOffset = _pivot.y - (_xIndexOffset - _pivot.x);

                for (int dy = 0; dy < h; dy++)
                {
                    for (int dx = 0; dx < w; dx++)
                    {
                        GameObject _texTile = GetTexTile(_tex, dx, w - dy - 1);

                        if (_texTile != null) Instantiate(_texTile, new Vector2(dx + _xRotIndexOffset, dy + _yRotIndexOffset) * tileSeperation, Quaternion.identity, mapParent);
                    }
                }
                break;
            case 4:
                _xRotIndexOffset = _pivot.x - (_yIndexOffset - _pivot.y);
                _yRotIndexOffset = _pivot.y + (_xIndexOffset - _pivot.x);

                for (int dy = 0; dy < h; dy++)
                {
                    for (int dx = 0; dx < w; dx++)
                    {
                        GameObject _texTile = GetTexTile(_tex, h - dx - 1, dy);

                        if (_texTile != null) Instantiate(_texTile, new Vector2(dx + _xRotIndexOffset, dy + _yRotIndexOffset) * tileSeperation, Quaternion.identity, mapParent);
                    }
                }
                break;
            default:
                GenerateTexVisuals(_tex, _xRotIndexOffset, _yRotIndexOffset);
                break;
        }
    }

    public void GenerateTexVisuals(Texture2D _tex,int _xIndexOffset, int _yIndexOffset)
    {
        int h = _tex.height;
        int w = _tex.width;

        for (int dy = 0; dy < h; dy++)
        {
            for (int dx = 0; dx < w; dx++)
            {
                GameObject _texTile = GetTexTile(_tex, dx, dy);

                if(_texTile != null) Instantiate(_texTile, new Vector2(dx + _xIndexOffset, dy + _yIndexOffset) * tileSeperation, Quaternion.identity, mapParent);
            }
        }
    }
    */
    ColorMapping GetTexColorMapping(Texture2D _tex, int dx, int dy)
    {
        Color _pixelColor = _tex.GetPixel(dx, dy);
        if (_pixelColor.a == 0 || _pixelColor.Equals(nullColor)) return null;

        for (int i = 0; i < colorMappings.Length; i++)
        {
            if (colorMappings[i].color.Equals(_pixelColor)) return colorMappings[i];
        }

        return null;
    }

    protected GameObject GetMappedPrefab(int _prefabID)
    {
        for (int i = 0; i < colorMappings.Length; i++)
        {
            if (colorMappings[i].prefabID == _prefabID) return colorMappings[i].prefab;
        }
        return null;
    }
}
