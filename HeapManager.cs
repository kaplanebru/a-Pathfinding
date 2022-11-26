using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeapManager : MonoBehaviour
{
    public static HeapManager Instance;
    void Awake()
    {
        Instance = this; 
    }
    
     public void ReorderAdd(List<Cell> pool)
   {
       if (pool.Count < 2) return;
       while (true)
       {
           int lastIndex = pool.Count-1;

           int parentIndex = (lastIndex - 1) / 2;
           if (SmallerFCost(parentIndex, lastIndex, pool))
               SwapCells(parentIndex, lastIndex, pool);
           else break;
       }
   }

    Vector2Int GetChildrenIndex(int parentIndex) => new (parentIndex * 2 + 1, parentIndex * 2 + 2);


    int SmallestChildIndex(int rightIndex, int leftIndex, List<Cell> pool)
    {
        return SmallerFCost(rightIndex, leftIndex, pool) ? rightIndex : leftIndex;
    }
   
    public void ReorderRemove(List<Cell> pool)
    {
        pool[0] = pool.Last();
        int poolCount = pool.Count;
        int indexToSwap = 0;

        while (true)
        {
            int leftIndex = GetChildrenIndex(indexToSwap).x;
            int rightIndex = GetChildrenIndex(indexToSwap).y;
            int oldIndexToSwap = indexToSwap;
            
            if (rightIndex >= poolCount)
            {
                if (leftIndex >= poolCount)
                    break;

                indexToSwap = leftIndex;                                       
            }
            else
            {
                indexToSwap = SmallestChildIndex(rightIndex, leftIndex, pool);
            }


            if (SmallerFCost(indexToSwap, oldIndexToSwap, pool))
                break;
            
            SwapCells(indexToSwap, oldIndexToSwap, pool);

        }
    }
    
    
    bool SmallerFCost(int index1, int index2, List<Cell> pool)
    {
        if (pool[index1].fCost < pool[index2].fCost)
            return true;
        
        if (Equal(index1, index2, pool))
            if (HCostSmaller(index1, index2, pool)) return true;
        
        return false;
    }

    bool Equal(int index1, int index2, List<Cell> pool)
    {
        return pool[index1].fCost == pool[index2].fCost;
    }

    bool HCostSmaller(int index1, int index2, List<Cell> pool)
    {
        return pool[index1].hCost < pool[index2].hCost;
    }
    
    
    void SwapCells(int index1, int index2, List<Cell> pool)
    {
        (pool[index1], pool[index2]) = (pool[index2], pool[index1]);
    }

    public void Swap(Cell cell1, Cell cell2)
    {
        (cell1, cell2) = (cell2, cell1);
    }

    
}
