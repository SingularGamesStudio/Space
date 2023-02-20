using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsSystem : MonoBehaviour
{
    //TODO: automatize, make private
    public List<Planet> planets= new List<Planet>();
    public List<PhysicsObject> addedObjects = new List<PhysicsObject>();
    void FindBounds(PhysicsObject obj, Tree t = null, bool safe = false)
    {
        if (safe && obj.GetIntersection(t.GetRect()) == PhysicsObject.Intersection.Inside) {
            return;
        }
        if (t == null) {
            foreach (Planet p in planets) {
                PhysicsObject.Intersection coll = obj.GetIntersection(p.GetRect());
                if (coll == PhysicsObject.Intersection.Inside) {
                    t = p.Root;
                    break;
                }
            }
            if (t == null)
                throw new System.NotImplementedException("Objects outside of planet bounds not supported yet");
        } else {
            t.objects.Remove(obj);
			t.objectsCnt--;
		}
        while (t.Parent != null && obj.GetIntersection(t.GetRect()) != PhysicsObject.Intersection.Inside) {
            t = t.Parent;
            t.objectsCnt--;
        }
        while (t.Size>1) {
            if (t.Color != null) {
                t.InitChildren(t.Color);
                t.Color = null;
            }
            bool ok = false;
            foreach(Tree child in t.Children) {
                if (obj.GetIntersection(child.GetRect()) == PhysicsObject.Intersection.Inside) {
                    ok = true;
                    t.objectsCnt++;
                    t = child;
                    break;
                }
            }
            if (!ok)
                break;
        }
        t.objectsCnt++;
        t.objects.Add(obj);
    }

    private void moveAll(Tree t)
    {
        foreach(PhysicsObject obj in t.objects) {
            obj.PhysicsTick();
        }
        if (t.objectsCnt > t.objects.Count) {
            foreach(Tree child in t.Children) {
                moveAll(child);
            }
        }
    }

	private void updateParents(Tree t)
	{
		foreach (PhysicsObject obj in t.objects) {
            FindBounds(obj, t, true);
		}
		if (t.objectsCnt > t.objects.Count) {
			foreach (Tree child in t.Children) {
				updateParents(child);
			}
		}
	}

    private void evalCollisionPlanet(Tree t, PhysicsObject obj)
    {
        obj.unmove();
    }
	private void evalCollision(PhysicsObject obj1, PhysicsObject obj2)
	{
		obj1.unmove();
		obj2.unmove();
	}

	private void Collide(Tree t,Stack<PhysicsObject> objects) //TODO:bottleneck, optimize as much as possible
    {
		foreach (PhysicsObject obj1 in t.objects) {
			objects.Push(obj1);
		}
		foreach (PhysicsObject obj1 in t.objects) {
			foreach (PhysicsObject obj2 in objects) {
                if (obj1!=obj2 && obj1.GetIntersection(obj2) != PhysicsObject.Intersection.None)
                    evalCollision(obj1, obj2);
			}
		}
		if (t.Color != null) {
			if (t.Color.Active) {
                foreach (PhysicsObject obj2 in objects) {
                    if (obj2.GetIntersection(t.GetRect()) != PhysicsObject.Intersection.None)
                        evalCollisionPlanet(t, obj2);
				}
			}
		} else {
            foreach(Tree child in t.Children) {
                Collide(child, objects);
            }
        }
		for(int i = 0; i<t.objects.Count; i++) {
            objects.Pop();
		}
	}

    void FixedUpdate()
    {
        foreach (PhysicsObject obj in addedObjects)
        {
            FindBounds(obj);
            obj.ApplyForce(new Vector2(-0.001f, 0), obj.massCenter);//!!!
        }
        addedObjects.Clear();
        foreach(Planet p in planets) {
            moveAll(p.Root);
            updateParents(p.Root);
            Collide(p.Root, new Stack<PhysicsObject>());
            updateParents(p.Root);
        }
    }
}
