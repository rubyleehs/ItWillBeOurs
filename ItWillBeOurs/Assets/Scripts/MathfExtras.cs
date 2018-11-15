using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathfExtras : MonoBehaviour
{
    public readonly static int[] primes = new int[10] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 29 };

    public Vector3 AlterVector(Vector3 vector, float angleChange) //works
    {
        if (angleChange == 0 || angleChange == float.NaN) return vector;
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angleChange), Vector3.one);
        vector = matrix.MultiplyPoint3x4(vector);
        return vector;
    }

    public Vector3 GetBezierCurvePoint(List<Vector3> bezierNodes, float t)
    {
        if (bezierNodes == null || bezierNodes.Count == 0)
        {
            ////Debug.Log("Cannot Form Bezier Curve With No Point, returning Vector3.Zero");
            return Vector3.zero;
        }
        if (bezierNodes.Count == 1)
        {
            ////Debug.Log("Cannot Form Bezier Curve With Single Point, Returning Original Point");
            return bezierNodes[0];
        }
        if (t > 1 || t < 0)
        {
            ////Debug.Log("Start/End Points of Bezier Outside Range. t value should be between 0 and 1. May have error");
        }

        List<Vector3> _finalPoints = new List<Vector3>();
        _finalPoints.AddRange(bezierNodes);
        List<Vector3> _nodes = new List<Vector3>();

        while (_finalPoints.Count > 1)
        {
            _nodes.Clear();
            _nodes.AddRange(_finalPoints);
            _finalPoints.Clear();
            for (int i = 0; i < _nodes.Count - 1; i++)
            {
                _finalPoints.Add(Vector3.Lerp(_nodes[i], _nodes[i + 1], t));
            }

        }
        return _finalPoints[0];
    }

    public int RandomValue(Vector2Int _range)
    {
        return Random.Range(_range.x, _range.y + 1);
    }
    public float RandomValue(Vector2 _range)
    {
        return Random.Range(_range.x, _range.y);
    }


    public float Get2DLookRotAngle(Vector2 _from, Vector2 _to)
    {
        Vector2 _dir = new Vector2(_to.x - _from.x, _to.y - _from.y);
        float _lookRotAngle = Mathf.Rad2Deg * Mathf.Atan(_dir.y / _dir.x) + 90;
        if (_dir.x == 0)
        {
            if (_dir.y > 0) _lookRotAngle = 0;
            else _lookRotAngle = 180;
        }
        if (_dir.x > 0) _lookRotAngle += 180;

        if (_lookRotAngle > 360) _lookRotAngle -= 360;
        else if (_lookRotAngle < 0) _lookRotAngle += 360;
        return _lookRotAngle;
    }

    public Vector2[] GetPointsAroundOrigin(Vector2 _origin, int _noOfPoints, float _radius, float _maxAngleHalfSpread, float _centerAngle)
    {
        float _angularDiff = 2f * _maxAngleHalfSpread / _noOfPoints;

        Vector2[] _points = new Vector2[_noOfPoints];

        for (int i = 0; i < _noOfPoints; i++)
        {
            _points[i] = AlterVector(Vector3.up * _radius, _centerAngle - _maxAngleHalfSpread + i * _angularDiff);
            _points[i] += _origin;
        }

        return _points;
    }

    public Vector3[] GetPointsPlusLookRotAroundOrigin(Vector2 _origin, int _noOfPoints, float _radius, float _maxAngleHalfSpread, float _centerAngle)
    {
        float _angularDiff = 2f * _maxAngleHalfSpread / _noOfPoints;

        Vector3[] _points = new Vector3[_noOfPoints];

        for (int i = 0; i < _noOfPoints; i++)
        {
            _points[i] = AlterVector(Vector3.up * _radius, _centerAngle - _maxAngleHalfSpread + i * _angularDiff);
            _points[i] += (Vector3)_origin;
            _points[i].z = _centerAngle - _maxAngleHalfSpread + i * _angularDiff;
        }

        return _points;
    }

    public Vector3Int RoundToVector3Int(Vector3 _v3)
    {
        int x = Mathf.RoundToInt(_v3.x);
        int y = Mathf.RoundToInt(_v3.y);
        int z = Mathf.RoundToInt(_v3.z);

        return new Vector3Int(x, y, z);
    }
    public Vector2Int RoundToVector2Int(Vector2 _v2)
    {
        int x = Mathf.RoundToInt(_v2.x);
        int y = Mathf.RoundToInt(_v2.y);

        return new Vector2Int(x, y);
    }

    public List<Vector2Int> GetIntOnlyLinePoints(Vector2Int _from, Vector2Int _to)
    {  
        List<Vector2Int> _points = new List<Vector2Int>();
        int dx = _to.x - _from.x;
        int dy = _to.y - _from.y;
        Vector2Int cal;

        if (dx == 0)
        {
            if(dy < 0)
            {
                cal = _to;
                _to = _from;
                _from = cal;
            }
            for (int y = _from.y; y <= _to.y; y++)
            {
                _points.Add(new Vector2Int(_from.x, y));
            }
        }
        else
        {
            if (_from.x > _to.x)
            {
                cal = _to;
                _to = _from;
                _from = cal;
                dx *= -1;
                dy *= -1;
            }

            float m = Mathf.Abs((float)dy / (float)dx);
            Debug.Log(m + " = " + dy + "/ " + dx);
            float error = 0;
            int y = _from.y;
            for (int x = _from.x; x <= _to.x; x++)
            {
                _points.Add(new Vector2Int(x, y));
                error += m;
                while (error >= 0.5f)
                {
                    y += Mathf.Abs(dy)/dy;
                    Debug.Log(x + " , " + y);
                    error -= 1;
                    if(error >= 0.5f && y < Mathf.Max(_from.y,_to.y) && y > Mathf.Min(_from.y,_to.y)) _points.Add(new Vector2Int(x, y));
                }
            }
        }
        /*
        for (int i = 0; i < _points.Count; i++)
        {
            Debug.Log(_points[i]);
        }*/
        return _points;
    }
}