using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapCard : CardDatastructure
{
    private ArrayList heap = new ArrayList();
    public override void Add(int num)
    {
        int index = heap.BinarySearch(num);
        if (index < 0)
            heap.Insert(~index, num);
        else
            heap.Insert(index, num);
    }

    public override int? Remove()
    {
        if (heap.Count == 0)
            return null;

        int ret = (int)heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);
        return ret;
    }

    public override int? Peek()
    {
        if (heap.Count == 0)
            return null;

        return (int)heap[heap.Count - 1];
    }

    public override void ClearData()
    {
        heap = new ArrayList();
    }
}
