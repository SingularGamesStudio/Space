using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sum : Func
{
    public Sum() {
        argCnt = 2;
    }
    public override float get(Vector2 pos) {
        return pos.x + pos.y;
    }
}
