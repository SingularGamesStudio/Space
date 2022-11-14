using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Coord : Func {
    public enum Axis {
        X_Polar,
        Y_R_Polar,
        Y_Polar,
        X_Decart,
        Y_Decart,
        X_Biome
    }
    public Axis axis;
    public float FreqMin;
	public float FreqMax;
    [HideInInspector]
	public float freq;
    
    public Coord(){
        argCnt = 0;
    }
    protected override float getSelf(FuncPassType args) {
        switch (axis) {
            case Axis.X_Polar:
                return (Mathf.Atan2(args.y, args.x) + Mathf.PI) * args.r*freq;
            case Axis.Y_R_Polar:
                return (Mathf.Sqrt(args.x * args.x + args.y * args.y)-args.r)*freq;
            case Axis.Y_Polar:
                return Mathf.Sqrt(args.x * args.x + args.y * args.y)*freq;
            case Axis.X_Decart:
                return args.x*freq;
            case Axis.Y_Decart:
                return args.y*freq;
            case Axis.X_Biome:
                return ((Mathf.Atan2(args.y, args.x) + Mathf.PI) * args.r - args.biomeCenter)*freq;
            default:
                return 0;
        }
    }

    protected override void InitSelf(int seed) {
		System.Random rnd = new System.Random(seed);
		freq = (float)rnd.NextDouble() * (FreqMax - FreqMin) + FreqMin;
	}
	protected override Func DeepCopySelf()
	{
		Coord res = new Coord();
		res.axis = (Axis)((int)axis);
        res.freq = freq;
		res.FreqMin = FreqMin;
		res.FreqMax = FreqMax;
		return res;
	}
}
