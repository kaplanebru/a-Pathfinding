using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Curve
{
    public Vector2Int pointA;
    public Vector2Int pointB;
    public Vector2Int pointC;
    public List<Vector2> curvePoints;
    
    public Curve(Vector2Int _pointA, Vector2Int _pointB, Vector2Int _pointC)
    {
        pointA = _pointA;
        pointB = _pointB;
        pointC = _pointC;
    }
    
    Vector2 CurvePoint(float t)
    {
    
        Vector2 AB = Vector2.Lerp(pointA, pointB, t);
        Vector3 BC = Vector2.Lerp(pointB, pointC, t);
        Vector2 curvePoint = Vector3.Lerp(AB, BC, t);
        return curvePoint;
    }
    
    public List<Vector2> GetCurvePoints(float amount)
    {
        float ratio = 1f / amount;
        curvePoints = new();
        float t = 0;
        while (t < 1) 
        {
            t = Mathf.MoveTowards(t, 1, ratio);
            curvePoints.Add(CurvePoint(t));
        }
        return curvePoints;
    }

    
    public float CurveDistance()
    {
        float curveDistance = 0;
        for (int i = 0; i < curvePoints.Count-1; i++)
        {
            var distance = Vector2.Distance(curvePoints[i], curvePoints[i + 1]);
            curveDistance += distance;
        }

        Debug.Log(curveDistance);
        return curveDistance;
    }

}
