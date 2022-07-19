using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePoint {
    public Vector2Int Point;
    public Vector2Int Dir;
    public EdgePoint(Vector2Int Point, Vector2Int Dir) {
        this.Point = Point;
        this.Dir = Dir;
    }
    public EdgePoint Copy() {
        return new EdgePoint(this.Point, this.Dir);
    }
}
