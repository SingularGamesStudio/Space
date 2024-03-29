using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perlin : Func
{
	public Perlin()
    {
        argCnt = 2;
    }
    protected override float getSelf(FuncPassType args)
    {
        return 2f*Mathf.PerlinNoise(args.x, args.y)-1f;
    }
    protected override Func DeepCopySelf()
    {
        Perlin perlin = new Perlin();
		return perlin;
	}
    protected override void InitSelf(int seed)
    {
    }
}
