using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector2Int pos;
    
    public int hCost, gCost, stoppingByGCost;
    public int fCost => gCost + hCost;

    public Cell(Vector2Int _pos)
    {
         pos = _pos;
    }
}


