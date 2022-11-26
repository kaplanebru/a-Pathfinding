using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CurvedRoute
{
    private List<Vector2Int> route;
    public CurvedRoute(List<Vector2Int> _route)
    {
        route = _route;
    }
     bool Diagonal(int i)
    {
        var distance = Vector2Int.Distance(route[i-1], route[i]); //var distance = 
        if(newCell.Count>0)
            newCell.Clear();
        return distance >= 1.4f;
    }
     
    public List<Vector2> SetCurves()
    {
        List<Vector2> newRoute = new();
        for (int i = 1; i < route.Count; i++) //0
        {
            if (Diagonal(i))
            {
                if (CheckObstacleOnRoute(i))
                {
                    InsertObstacleCell(i);
                    InsertCurve(i, newRoute);
                    i += 2;
                    continue;
                }
                
                if (OverIndex(i, newRoute))
                    break;
                
                if(!Diagonal(i+1))
                {
                    InsertCurve(i, newRoute);
                    i++;
                }
                else
                    newRoute.Add(route[i-1]);
            }
            
            else
            {
                if (OverIndex(i, newRoute))
                    break;

                if (Diagonal(i + 1))
                {
                    if (CheckObstacleOnRoute(i+1))
                    {
                        InsertObstacleCell(i+1);
                        InsertCurve(i+1, newRoute);
                        i += 3;
                        continue;
                    }
                    InsertCurve(i, newRoute);
                    i++;
                }
                else
                    newRoute.Add(route[i-1]);
            }
            
        }
        newRoute.Add(route.Last());
        List<Vector2> temp = newRoute.Distinct().ToList();
        newRoute.Clear();
        newRoute = temp;
   
        return newRoute;

    }

    void InsertObstacleCell(int i)
    {
        route.Insert(i, newCell.Last());
            newCell.Clear();
           
    }

    void InsertCurve(int i, List<Vector2> newRoute)
    {
        if (OverIndex(i, newRoute))
            return;
        Curve newCurve = new Curve( route[i-1], route[i], route[i+1]);  
        var curvePoints = newCurve.GetCurvePoints(10);
      

        for (int j = 0; j < curvePoints.Count - 1; j++) 
        {
            newRoute.Add(curvePoints[j]);
        }
    }

    bool OverIndex(int i, List<Vector2> newRoute)
    {
        if (i + 1 == route.Count)
        {
            newRoute.Add(route[i-1]);
            return true;
        }
        return false;
    }
    
    private List<Vector2Int> newCell = new ();
    bool CheckObstacleOnRoute(int i)
    {
        Vector2 dir = (route[i] - route[i - 1]);
        dir = dir.normalized;
        var newDir = Vector2Int.RoundToInt(dir);
        Vector2Int current = route[i - 1];
        List<Vector2Int> toCheck = new (); // new Vector2Int[2];
        switch (newDir.x)
        {
            case 1 when newDir.y == 1:
                toCheck.Add( new Vector2Int(current.x, current.y + 1));
                toCheck.Add(new Vector2Int(current.x + 1, current.y));
                break;
            case -1 when newDir.y == -1:
                toCheck.Add(new Vector2Int(current.x, current.y - 1));
                toCheck.Add(new Vector2Int(current.x - 1, current.y));
                break;
            case -1 when newDir.y == 1:
                toCheck.Add(new Vector2Int(current.x-1, current.y));
                toCheck.Add(new Vector2Int(current.x, current.y+1));
                break;
            case 1 when newDir.y == -1:
                toCheck.Add(new Vector2Int(current.x, current.y-1));
                toCheck.Add(new Vector2Int(current.x+1, current.y));
                break;
        }

        bool hasObstacle = false;
        for (int j = toCheck.Count - 1; j >= 0; j--)
        {
            
            if (!GameState.IsFree(toCheck[j])) //
            {
                hasObstacle = true;
                toCheck.Remove(toCheck[j]);
            }
        }

        if (hasObstacle && toCheck.Count > 0)
        {
            newCell.Add(toCheck[0]);
            return true;
        
        }
        
        return false;

    }
    
    
}
