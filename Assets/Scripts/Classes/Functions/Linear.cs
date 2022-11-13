using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Linear : Func
{
    public float kMin;
    public float kMax;
    public float bMin;
    public float bMax;
    [HideInInspector]
    public float k;
    [HideInInspector]
    public float b;
    public Linear() {
        argCnt = 1;
    }
    protected override float getSelf(FuncPassType args) {
        return k * args.x + b;
    }
    protected override void InitSelf(int seed) {
        System.Random rnd = new System.Random(seed);
        k = (float)rnd.NextDouble() * (kMax - kMin) + kMin;
        b = (float)rnd.NextDouble() * (bMax - bMin) + bMin;
    }
}
