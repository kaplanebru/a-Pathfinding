using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

/// <summary>
/// Provides pathfinding functionality.
/// </summary>
public class Router
{
    /// <summary>
    /// Find the route between two cells on the grid.
    /// </summary>
  
    public List<Cell> pool = new (); 
    HashSet<Vector2Int> visited = new(); // Visited cells. 
    public Dictionary<Vector2Int, Vector2Int> parents = new(); // Tracks where did we reach a given cell from.
    Cell bestCell;
    private Cell start, end;
    

    public Router(Vector2Int _start, Vector2Int _end)
    {
        start = new Cell(_start);
        end = new Cell(_end);
    }
    

    public List<Vector2> FindRoute() 
    {
        if (!GameState.IsFree(end.pos))
            return null; // Principially not possible to reach the end.

        // 1. Traverse the grid until you find the end.
        StartPool();
        while (pool.Count > 0)
        {
            SetBestCell();
            visited.Add(bestCell.pos);
            RemoveFromHeap();
            Cell[] candidates = new Cell[8];
            FillCandidatesList(candidates);
            ManagePool(candidates);

            if (parents.ContainsKey(end.pos))
                break;
        }
        return DrawRoute();
    }
    

    void StartPool()
    {
        start.hCost = Distance(end, start);
        start.gCost = 0;
        bestCell = start;
        pool.Add(start);
    }

    void SetBestCell()
    {
        bestCell = pool[0];
    }

    void FillCandidatesList(Cell[] candidates)
    {
        int index = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                Cell candidate = new Cell(new Vector2Int(bestCell.pos.x + j, bestCell.pos.y + i));
                if(candidate==null) 
                    continue;
               
                candidate.stoppingByGCost =  Distance(bestCell, candidate) + bestCell.gCost;
                UpdateCandidate(candidate);
                candidates[index] = candidate;
                index++;
            }
        }
    }

    
    void ManagePool(Cell[] candidates)
    {
        foreach (var candidate in candidates)
        {
           if (!GameState.IsFree(candidate.pos) || visited.Contains(candidate.pos))
                continue;

           if (candidate == end)
                break;
           
           AddToPool(candidate);
        }
        pool = pool.Distinct().ToList();
    }

    bool NeedsUpdate( Cell candidate) => candidate.stoppingByGCost < candidate.gCost;

    void UpdateCandidate(Cell candidate)
    {
        if (pool.Contains(candidate) && NeedsUpdate(candidate))
        {
            candidate.gCost = candidate.stoppingByGCost;
            HeapManager.Instance.Swap(candidate, pool.Last());
            HeapManager.Instance.ReorderAdd(pool);
        }
            
    }
    
    
    void AddToPool(Cell candidate)
    {
        
        if (!pool.Contains(candidate))
        {
            candidate.gCost = candidate.stoppingByGCost;
            candidate.hCost = Distance(candidate, end);

            if (!LowerFCostThanBestcell(candidate)) return;
            pool.Add(candidate);
            TrackPath(candidate);
        }
        HeapManager.Instance.ReorderAdd(pool);

    }
    

    void RemoveFromHeap()
    {
        if(pool.Count > 1)
            HeapManager.Instance.ReorderRemove(pool);
        pool.Remove(bestCell);
    }
    

    bool LowerFCostThanBestcell(Cell cell)
    {
        return cell.fCost < bestCell.fCost || (cell.fCost == bestCell.fCost && cell.hCost < bestCell.hCost);
    }
    
    int Distance(Cell cell1, Cell cell2)
    {
        Vector2Int distances = new Vector2Int(
            Mathf.Abs(cell1.pos.x - cell2.pos.x), 
            Mathf.Abs(cell1.pos.y - cell2.pos.y));

        int diagonal = 14 * Mathf.Min(distances.x, distances.y);
        int straight = 10 * Mathf.Abs(distances.x - distances.y);
        return diagonal + straight;
    }

    void TrackPath(Cell cell)
    {
        parents[cell.pos] = bestCell.pos;
    }

    List<Vector2> DrawRoute()
    {
        // 2. Assemble the route.
        if (!parents.ContainsKey(end.pos))  
            return null;

        var route = new List<Vector2Int>();
        Vector2Int cell = end.pos;
        
        while (cell != start.pos)  
        {
            route.Insert(0, cell);
            if (!parents.ContainsKey(cell)) break;
            cell = parents[cell];   
        }
        route.Insert(0, start.pos);
        var newRoute = new CurvedRoute(route).SetCurves()?.ToList();
        return newRoute;
    }
    

}
