using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : PhysicsObject
{
    public float size = 0;
    public override Intersection GetIntersection(Rect rect)
    {
        Rect r = new Rect(transform.position, new Vector2(size, size));
        int cnt = 0;
        throw new System.NotImplementedException();
    }

    public override Intersection GetIntersection(PhysicsObject other)
    {
        throw new System.NotImplementedException();
    }

    public override float Size()
    {
        throw new System.NotImplementedException();
    }
}
