﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterCard : CardDatastructure
{
    private int? register;
    public override void MoveTo(int num)
    {
        register = num;
    }

    public override int? MoveFrom()
    {
        int? ret = register;
        register = null;
        // Returns null if the register is empty.
        return ret;
    }

    public override int? Peek()
    {
        return register;
    }

    public override void ClearData()
    {
        register = null;
    }
}
