using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// May act as a Vector2, or itself
/// Vector2, on forward pass and when passed to functions with arguments
/// Itself, when passed to Coord Function
/// </summary>
public class FuncPassType
{
    public float x;
    public float y;
    public int r;
    public float biomeCenter;
    public FuncPassType(float x, float y, int r, float biomeCenter) {
        this.x = x;
        this.y = y;
        this.r = r;
        this.biomeCenter = biomeCenter;
    }
    public FuncPassType(float x, float y) {
        this.x = x;
        this.y = y;
    }
}
