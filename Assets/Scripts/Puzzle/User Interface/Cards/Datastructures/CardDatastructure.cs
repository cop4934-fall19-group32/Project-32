using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardDatastructure
{
    public abstract void MoveTo(int num);

    public abstract int? MoveFrom();

    public abstract int? Peek();

    public abstract void ClearData();
}
