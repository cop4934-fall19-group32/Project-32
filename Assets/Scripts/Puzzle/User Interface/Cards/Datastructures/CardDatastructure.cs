using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardDatastructure
{
    public abstract void Add(int num);

    public abstract int? Remove();

    public abstract int? Peek();

    public abstract void ClearData();
}
