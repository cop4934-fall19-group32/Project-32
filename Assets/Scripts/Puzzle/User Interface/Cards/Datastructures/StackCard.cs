using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackCard : CardDatastructure
{
    private Stack<int> stack = new Stack<int>();
    public override void Add(int num)
    {
        stack.Push(num);
    }

    public override int? Remove()
    {
        if (stack.Count == 0)
            return null;

        return stack.Pop();
    }

    public override int? Peek()
    {
        if (stack.Count == 0)
            return null;

        return stack.Peek();
    }

    public override void ClearData()
    {
        stack = new Stack<int>();
    }
}
