using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public abstract class PhysicsObject : MonoBehaviour
{
    
    protected float angularVelocity = 0;
	protected float torque = 0;
	protected Vector2 force = 0;
	protected Vector2 velocity = 0;
	protected Vector2 massCenter;
    public float Mass;

    private void Awake()
    {
        massCenter = transform.position;
    }
    public void ApplyForce(Vector2 force, Vector2 pos)
    {
        Vector2 r = pos - massCenter;
        torque += Utils.CrossProduct(r, force);
        this.force += force;
    }

    protected abstract void move();

    private void FixedUpdate()
    {
        angularVelocity += torque * Time.fixedDeltaTime;
        velocity += (force / Mass) * Time.fixedDeltaTime;
        move();
    }
}
