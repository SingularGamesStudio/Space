using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Func
{
    [SerializeReference]
    public Func arg1 = null;
    [SerializeReference]
    public Func arg2 = null;
    public int argCnt;
    public float PropertySize;
    public float getRec(Vector2 pos) {
        if (arg1 == null) {
            return get(pos);
        }
        else if (arg2 == null) {
            return get(new Vector2(arg1.getRec(pos), 0));
        }
        else {
            return get(new Vector2(arg1.getRec(pos), arg2.getRec(pos)));
        }
    }
    public abstract float get(Vector2 args);
}
