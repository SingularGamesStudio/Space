using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Linear : Func
{
    public float KMin;
    public float KMax;
    public float BMin;
    public float BMax;
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
        k = (float)rnd.NextDouble() * (KMax - KMin) + KMin;
        b = (float)rnd.NextDouble() * (BMax - BMin) + BMin;
    }
	protected override Func DeepCopySelf()
	{
		Linear res = new Linear();
		res.KMin = KMin;
		res.KMax = KMax;
		res.BMin = BMin;
		res.BMax = BMax;
		res.k = k;
		res.b = b;
		return res;
	}
}
