using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Serializable]
public struct CachedCard
{
    public string address;

    public CachedCard(string address)
    {
        this.address = address;
    }
}
