using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class PhysicsObject : MonoBehaviour
{
    public enum Intersection
    {
        Inside,
        Partial,
        None
    }
    protected float angularVelocity = 0;
	protected float torque = 0;
	protected Vector2 force = Vector2.zero;
	protected Vector2 velocity = Vector2.zero;
	protected Vector2 massCenter;
    public float mass;
    public void ApplyForce(Vector2 force, Vector2 pos)
    {
        Vector2 r = pos - massCenter;
        torque += Utils.CrossProduct(r, force);
        this.force += force;
    }

    public abstract Intersection GetIntersection(Rect rect);

    public abstract float Size();

    private void FixedUpdate()
    {
        angularVelocity += torque * Time.fixedDeltaTime;
        velocity += (force / mass) * Time.fixedDeltaTime;
        //move();
    }
}
