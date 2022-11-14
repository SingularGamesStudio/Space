using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Func
{
    [SerializeReference]
    public Func arg1 = null;
    [SerializeReference]
    public Func arg2 = null;
    [HideInInspector]
    public int argCnt;
    public float get(FuncPassType pos) {
        if (arg1 == null) {
            return getSelf(pos);
        }
        else if (arg2 == null) {
            return getSelf(new FuncPassType(arg1.get(pos), 0));
        }
        else {
            return getSelf(new FuncPassType(arg1.get(pos), arg2.get(pos)));
        }
    }
    
    public void Init(int seed) {
        InitSelf(seed);
        System.Random rnd = new System.Random(seed);
        if (argCnt == 2) {
            arg1.Init(rnd.Next(1000000));
            arg2.Init(rnd.Next(1000000));
        }
        else if (argCnt == 1) {
            arg1.Init(rnd.Next(1000000));
        }
    }
    
    public Func DeepCopy() {
        Func res = DeepCopySelf();
        res.arg1 = null;
        res.arg2 = null;
		if(arg1!=null)
		    res.arg1 = arg1.DeepCopy();
		if (arg2 != null)
			res.arg2 = arg2.DeepCopy();
        return res;
	}

    protected abstract Func DeepCopySelf();
	
    protected abstract void InitSelf(int seed);
    
    protected abstract float getSelf(FuncPassType args);
}
