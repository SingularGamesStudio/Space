using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    //TODO: automatize, make private
    public List<Planet> planets= new List<Planet>();
    public List<PhysicsObject> addedObjects = new List<PhysicsObject>();
    Dictionary<Tree, List<PhysicsObject>> objects = new Dictionary<Tree, List<PhysicsObject>>();
    bool FindBounds(PhysicsObject obj, Tree t = null)
    {
        bool retVal = true;
        if (t == null)
        {
            foreach (Planet p in planets)
            {
                PhysicsObject.Intersection coll = obj.GetIntersection(p.GetRect());
                if (coll == PhysicsObject.Intersection.Inside)
                {
                    t = p.Root;
                    break;
                } else if(coll == PhysicsObject.Intersection.Partial)
                {
                    t = p.Root;
                    retVal = false;
                    break;
                }
            }
        }
        while(obj.GetIntersection(t.GetRect())!=PhysicsObject.Intersection.Inside && t.Parent != null)
        {
            t = t.Parent;
        }
        if(t.Color!= null)
        {

        } else
        {

        }
        return retVal;
    }
    private void FixedUpdate()
    {
        List<PhysicsObject> newNull = new List<PhysicsObject>();
        foreach (PhysicsObject obj in objects[null])
        {
            if(!FindBounds(obj))
                newNull.Add(obj);
        }
        objects[null] = newNull;
        foreach (PhysicsObject obj in addedObjects)
        {
            if (!FindBounds(obj))
            {
                objects[null].Add(obj);
            }
        }
        addedObjects.Clear();

    }

}
