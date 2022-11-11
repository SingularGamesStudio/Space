using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Linear : Func
{
    public int k;
    public int b;
    public Linear() {
        argCnt = 1;
    }
    public override float get(Vector2 pos) {
        return k * pos.x + b;
    }
}
