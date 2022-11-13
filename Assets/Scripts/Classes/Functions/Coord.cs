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
    
    public Coord(){
        argCnt = 0;
    }
    protected override float getSelf(FuncPassType args) {
        switch (axis) {
            case Axis.X_Polar:
                return (Mathf.Atan2(args.y, args.x) + Mathf.PI) * args.r;
            case Axis.Y_R_Polar:
                return Mathf.Sqrt(args.x * args.x + args.y * args.y)-args.r;
            case Axis.Y_Polar:
                return Mathf.Sqrt(args.x * args.x + args.y * args.y);
            case Axis.X_Decart:
                return args.x;
            case Axis.Y_Decart:
                return args.y;
            case Axis.X_Biome:
                return (Mathf.Atan2(args.y, args.x) + Mathf.PI) * args.r - args.biomeCenter;
            default:
                return 0;
        }
    }

    protected override void InitSelf(int seed) {}
}
