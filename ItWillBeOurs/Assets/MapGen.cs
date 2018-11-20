using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegionPoolData
{
    public int maxSize;
    public Texture2D[] centerTex; //center overides all things. //1
    public Texture2D[] sideTex; //2
    public Texture2D[] cornerTex; // has to be a square //3
    //-1 is wall, 0 is empty
    public int[] cornerWallDepth;
}


public class MapGen : VisualGen {

    public Vector2Int mapRadius;
    //public Vector3Int[,] mapRegionData;
    public GameObject regionParent;

    [Header("Region Datas")]
    public int mainWarzoneRegionID;
    public RegionPoolData mainWarzoneRegionPoolData;

    private Vector2Int mapSize;

    private void Start()
    {
        Initialize();
        CreateRegion(0, 0, mainWarzoneRegionID, mainWarzoneRegionPoolData);
    }

    void Initialize()
    {
        mapSize = new Vector2Int(2 * mapRadius.x + 1, 2 * mapRadius.y + 1);
        //mapRegionData = new Vector3Int[mapSize.x,mapSize.y];
    }


    void CreateRegion(int x, int y, int _regionID, RegionPoolData _regionPoolData)
    {
        //Pick region meta Tex
        int[] regionMetaID = new int[4] { Random.Range(0, _regionPoolData.centerTex.Length), Random.Range(0, _regionPoolData.sideTex.Length),  Random.Range(0, _regionPoolData.cornerTex.Length), Random.Range(0, _regionPoolData.cornerTex.Length) };
        Texture2D _centerTex = _regionPoolData.centerTex[regionMetaID[0]];
        Texture2D _sideTex = _regionPoolData.sideTex[regionMetaID[1]];
        Texture2D[] _cornerTex = new Texture2D[2] { _regionPoolData.cornerTex[regionMetaID[2]], _regionPoolData.cornerTex[regionMetaID[3]] };
        int[] _cornerSequence = new int[4] { Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2) };
        int[] _cornerWallDepth = new int[2]{_regionPoolData.cornerWallDepth[regionMetaID[2]],  _regionPoolData.cornerWallDepth[regionMetaID[3]] };

        //Get associate subregion Size
        Vector2Int _centerTexSize = new Vector2Int(_centerTex.width,_centerTex.height);
        int _sideTexHeight = _sideTex.height;
        int[] _cornerTexSize = new int[2] { _cornerTex[0].width,_cornerTex[1].width };

        Vector2Int _regionSize = Vector2Int.one * (Random.Range(_regionPoolData.maxSize/2, _regionPoolData.maxSize) + 2);
        _regionSize.x = Mathf.Max(2 +_centerTexSize.x + Mathf.Max(_cornerTexSize[_cornerSequence[1]], _cornerTexSize[_cornerSequence[2]]) + Mathf.Max(_cornerTexSize[_cornerSequence[0]], _cornerTexSize[_cornerSequence[3]]), _regionSize.x);
        _regionSize.y = Mathf.Max(2 +_centerTexSize.y + Mathf.Max(_cornerTexSize[_cornerSequence[0]], _cornerTexSize[_cornerSequence[1]]) + Mathf.Max(_cornerTexSize[_cornerSequence[2]], _cornerTexSize[_cornerSequence[3]]), _regionSize.y);
        Vector2Int _regionCenterSize =_regionSize - new Vector2Int(Mathf.Max(_cornerTexSize[_cornerSequence[0]],_cornerTexSize[_cornerSequence[3]]) + Mathf.Max(_cornerTexSize[_cornerSequence[1]],_cornerTexSize[_cornerSequence[2]]), Mathf.Max(_cornerTexSize[_cornerSequence[0]], _cornerTexSize[_cornerSequence[1]]) + Mathf.Max(_cornerTexSize[_cornerSequence[2]], _cornerTexSize[_cornerSequence[3]]));
        Vector2Int _regionCenter = new Vector2Int(_regionCenterSize.x / 2, _regionCenterSize.y / 2) + new Vector2Int(Mathf.Max(_cornerTexSize[_cornerSequence[1]], _cornerTexSize[_cornerSequence[2]]), Mathf.Max(_cornerTexSize[_cornerSequence[2]], _cornerTexSize[_cornerSequence[3]]));

        int[,] _regionIntMap = new int[_regionSize.x, _regionSize.y];
        //SetAreaValue(ref _regionIntMap, Vector2Int.zero, _regionSize - Vector2Int.one, -1);//make border
        SetAreaValue(ref _regionIntMap, Vector2Int.one, _regionSize - Vector2Int.one * 2, 999);//set everything to unchanged

        SetAreaTexValue(_centerTex, ref _regionIntMap, _regionCenter - new Vector2Int(_centerTexSize.x / 2, _centerTexSize.y / 2), Random.value > 0.5f, Random.value > 0.5f, Random.value > 0.5f);//center
        
        SetAreaTexValue(_cornerTex[_cornerSequence[0]], ref _regionIntMap, _regionSize - Vector2Int.one * (_cornerTexSize[_cornerSequence[0]] + 2), 999, true, true, false);//TR corner
        SetAreaTexValue(_cornerTex[_cornerSequence[1]], ref _regionIntMap, new Vector2Int(1, _regionSize.y - _cornerTexSize[_cornerSequence[1]] - 2), 999, false, true, false);//TL corner
        SetAreaTexValue(_cornerTex[_cornerSequence[2]], ref _regionIntMap, Vector2Int.one, 999, false, false, false);//BL corner
        SetAreaTexValue(_cornerTex[_cornerSequence[3]], ref _regionIntMap, new Vector2Int(_regionSize.x - _cornerTexSize[_cornerSequence[3]] - 2, 1), 999, true, false, false); //BR corner
        
        //SetAreaValue(ref _regionIntMap, Vector2Int.one * (_cornerWallDepth + 2), _regionSize - Vector2Int.one * (_cornerWallDepth + 3), 999, 0);
        //SetAreaValue(ref _regionIntMap, Vector2Int.one * (_cornerWallDepth + 1), _regionSize - Vector2Int.one * (_cornerWallDepth + 2), 999, -1);
        
        List<Vector2Int> _sideBorders = new List<Vector2Int>();
        
        _sideBorders.AddRange(GetIntOnlyLinePoints(new Vector2Int(_regionSize.x - _cornerTexSize[_cornerSequence[0]] - 3, _regionSize.y - _cornerWallDepth[_cornerSequence[0]] - 3) , new Vector2Int(1 + _cornerTexSize[_cornerSequence[1]], _regionSize.y -_cornerWallDepth[_cornerSequence[1]] -3)));
        _sideBorders.AddRange(GetIntOnlyLinePoints(new Vector2Int(1 + _cornerWallDepth[_cornerSequence[1]], _regionSize.y - _cornerTexSize[_cornerSequence[1]] - 3), new Vector2Int(1 + _cornerWallDepth[_cornerSequence[2]], _cornerTexSize[_cornerSequence[2]] + 1)));
        _sideBorders.AddRange(GetIntOnlyLinePoints(new Vector2Int(1 + _cornerTexSize[_cornerSequence[2]], _cornerWallDepth[_cornerSequence[2]] + 1), new Vector2Int(_regionSize.x - _cornerTexSize[_cornerSequence[3]] - 3, _cornerWallDepth[_cornerSequence[3]] + 1)));
        _sideBorders.AddRange(GetIntOnlyLinePoints(new Vector2Int(_regionSize.x - _cornerWallDepth[_cornerSequence[3]] -3, _cornerTexSize[_cornerSequence[3]] + 1), new Vector2Int(_regionSize.x - _cornerWallDepth[_cornerSequence[0]] - 3, _regionSize.y - _cornerTexSize[_cornerSequence[0]] - 3)));

        for (int i = 0; i < _sideBorders.Count ; i++)
        {
            _regionIntMap[_sideBorders[i].x, _sideBorders[i].y] = -1;
        }

        for (int ry = 0; ry < _regionSize.y; ry++)//
        {
            for (int rx = 0; rx < _regionSize.x; rx++)
            {
                GameObject _mapObj = GetMappedPrefab(_regionIntMap[rx, ry]);
                if(_mapObj != null) Instantiate(_mapObj,new Vector2(x + rx, y +ry) * tileSeperation, Quaternion.identity, mapParent);
            }
        }
    }

    void SetAreaValue(ref int[,] _intMap, Vector2Int _fromIndex, Vector2Int _toIndex, int _fromValue, int _toValue)
    {
        for (int y = _fromIndex.y; y < _toIndex.y; y++)
        {
            for (int x = _fromIndex.x; x < _toIndex.x; x++)
            {
                if (_intMap[x, y] == _fromValue) _intMap[x, y] = _toValue;
            }
        }
    }
    void SetAreaValue(ref int[,] _intMap, Vector2Int _fromIndex, Vector2Int _toIndex, int _value)
    {
        for (int y = _fromIndex.y; y < _toIndex.y; y++)
        {
            for (int x = _fromIndex.x; x < _toIndex.x; x++)
            {
                _intMap[x, y] = _value;
            }
        }
    }

    bool IsWithinCorner(int _checkX, int _checkY, int _maxX, int _maxY, int _cornerSize)
    {
        int _count = 0;
        if (_checkX < _cornerSize) _count++;
        if (_checkY < _cornerSize) _count++;
        if (_checkX >= _maxX - _cornerSize) _count++;
        if (_checkY >= _maxY - _cornerSize) _count++;

        if (_count > 2) Debug.Log("Corner Overlap Warning");
        if (_count >= 2) return true;
        else return false;

    }
}
