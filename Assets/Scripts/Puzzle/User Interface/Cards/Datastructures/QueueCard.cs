using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueCard : CardDatastructure
{
    private Queue<int> queue = new Queue<int>();
    public override void MoveTo(int num)
    {
        queue.Enqueue(num);
    }

    public override int? MoveFrom()
    {
        if (queue.Count == 0)
            return null;

        return queue.Dequeue();
    }

    public override int? Peek()
    {
        if (queue.Count == 0) {
            return null;
        }
        else { 
            return queue.Peek();
        } 
    }

    public override void ClearData()
    {
        queue = new Queue<int>();
    }
}
