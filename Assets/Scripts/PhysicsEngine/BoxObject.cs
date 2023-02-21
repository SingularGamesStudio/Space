using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObject : PhysicsObject
{
    public float size = 0;
    public override Intersection GetIntersection(Rect other)
    {
        Rect self = new Rect((Vector2)transform.position - new Vector2(size/2, size/2), new Vector2(size, size));
        int cnt = 0;
        //Debug.Log("object: "+self);
		//Debug.Log("planet: " + other);
		//Debug.Log(transform.rotation.eulerAngles.z);
        List<Vector2> anglesSelf = Utils.rectPoints(self, self.center, transform.rotation.eulerAngles.z);
		List<Vector2> anglesOther = Utils.rectPoints(other, self.center, -transform.rotation.eulerAngles.z);
		foreach(Vector2 v in anglesSelf) {
            if (other.Contains(v))
                cnt++;
        }
        if (cnt == 4)
            return Intersection.Inside;
        if (cnt > 0)
            return Intersection.Partial;
		foreach (Vector2 v in anglesOther) {
			if (self.Contains(v))
				return Intersection.Partial;
		}
        return Intersection.None;
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
