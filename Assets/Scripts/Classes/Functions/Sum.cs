using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sum : Func {
    public Sum() {
        argCnt = 2;
    }
    protected override float getSelf(FuncPassType args) {
        return args.x + args.y;
    }
    protected override void InitSelf(int seed) { }
}
