using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct CachedCard
{
    public string type;

    public CachedCard(string t)
    {
        type = t;
    }
}
