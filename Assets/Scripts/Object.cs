using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Object : MonoBehaviour {
    public Tree Root;
    public int Size = 256;
    public bool DebugMode;
    public ObjectRenderer Renderer;
    
    void Start() {
        Root = new Tree(Size, this, new PixelState(0, true));
        Root.CircleFill(new Vector2(2048, 2048-100), 200, new PixelState(0, false));
        Root.CircleFill(new Vector2(604, 300), 110, new PixelState(0, false));
        Root.CircleFill(new Vector2(704, 500), 230, new PixelState(0, true));
        Root.TriangleFill(new Vector2(100, 510), new Vector2(435, 128), new Vector2(555, 666), new PixelState(0, false));
        Root.TriangleFill(new Vector2(345, 923), new Vector2(111, 111), new Vector2(654, 0), new PixelState(0, true));
    }
    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            Vector2 Pos = Utils.TransformPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform, Size);
            List<EdgePoint> Edge = GetEdge(new Vector2Int((int)Pos.x, (int)Pos.y), 300, 8);
            if (Edge != null) {
                List<float> Curve;
                //Curve = Curves.Curve_Square(100, 300);
                Curve = Curves.Curve_Perlin(25, 100, 300);
                List<Vector2> NewEdge = Curves.Shift(Edge, Curve);
                
                Transform(Edge, NewEdge);
            }
        }
    }

    public void Transform(List<EdgePoint> Edge, List<Vector2> NewEdge) {
        float Delta = ((float)Edge.Count) / (NewEdge.Count - 1);
        int Last = 0;
        List<Vector2> NewEdgeResized = new List<Vector2>();
        for (int i = 1; i < NewEdge.Count; i++) {
            int Now = (int)(i * Delta);
            if (i == NewEdge.Count - 1)
                Now = Edge.Count;
            for (int j = Last; j < Now; j++) {
                NewEdgeResized.Add((((float)(j - Last)) / (Now - Last)) * (NewEdge[i] - NewEdge[i - 1]) + NewEdge[i - 1]);
            }
            Last = Now;
        }
        for (int i = 0; i < 299; i++) {
            Debug.DrawLine(Utils.InverseTransformPos(Edge[i].Point, transform, Size), Utils.InverseTransformPos(Edge[i + 1].Point, transform, Size), new Color(255, 0, 255), 1000f);
        }
        for (int i = 0; i < 299; i++) {
            Debug.DrawLine(Utils.InverseTransformPos(NewEdgeResized[i], transform, Size), Utils.InverseTransformPos(NewEdgeResized[i + 1], transform, Size), new Color(255, 255, 0), 1000f);
        }
        for (int i = 1; i < Edge.Count-2; i++) {
            Debug.DrawLine(Utils.InverseTransformPos(Edge[i - 1].Point, transform, Size), Utils.InverseTransformPos(Edge[i].Point, transform, Size), new Color(255, 0, 0), 1000f);
            Debug.DrawLine(Utils.InverseTransformPos(Edge[i - 1].Point, transform, Size), Utils.InverseTransformPos(NewEdgeResized[i - 1], transform, Size), new Color(255, 0, 0), 1000f);
            Debug.DrawLine(Utils.InverseTransformPos(NewEdgeResized[i - 1], transform, Size), Utils.InverseTransformPos(Edge[i].Point, transform, Size), new Color(255, 0, 0), 1000f);
            Debug.DrawLine(Utils.InverseTransformPos(NewEdgeResized[i - 1], transform, Size), Utils.InverseTransformPos(NewEdgeResized[i], transform, Size), new Color(255, 0, 0), 1000f);
            Debug.DrawLine(Utils.InverseTransformPos(Edge[i].Point, transform, Size), Utils.InverseTransformPos(NewEdgeResized[i], transform, Size), new Color(255, 0, 0), 1000f);
            
            Root.TriangleFill(Edge[i - 1].Point, Edge[i].Point, NewEdgeResized[i - 1], new PixelState(0, true));
            Root.TriangleFill(NewEdgeResized[i - 1], NewEdgeResized[i], Edge[i].Point, new PixelState(0, true));
        }
    }

    private Vector2Int _getClosestEdge(Vector2Int Point, int R) {
        Vector2Int Ans = new Vector2Int(-1, -1);
        for (int x = -R; x< R; x++) {
            for (int y = -R; y < R; y++) {
                Vector2Int Shift = new Vector2Int(x, y);
                if (Renderer.GetPixel(Point+Shift).Active && Utils.Distance2(Ans, Point)> Utils.Distance2(new Vector2(x, y), Point)) {
                    if (!Renderer.GetPixel(Point + Shift+Data.Main.ShiftsLR[0]).Active || !Renderer.GetPixel(Point + Shift + Data.Main.ShiftsLR[1]).Active || !Renderer.GetPixel(Point + Shift + Data.Main.ShiftsLR[2]).Active || !Renderer.GetPixel(Point + Shift + Data.Main.ShiftsLR[3]).Active) {
                        Ans = Point + Shift;
                    }
                }
            }
        }
        return Ans;
    }

    List<EdgePoint> GetEdge(Vector2Int Point, int Length, int R) {
        Point = _getClosestEdge(Point, R);
        if (Point.x == -1 && Point.y == -1)
            return null;
        Vector2Int dir = new Vector2Int(0, 1);
        if(Renderer.GetPixel((Point+dir).x, (Point + dir).y).Active) {
            dir = new Vector2Int(0, -1);
            if (Renderer.GetPixel((Point + dir).x, (Point + dir).y).Active) {
                dir = new Vector2Int(1, 0);
                if (Renderer.GetPixel((Point + dir).x, (Point + dir).y).Active) {
                    dir = new Vector2Int(-1, 0);
                    if (Renderer.GetPixel((Point + dir).x, (Point + dir).y).Active) {
                        Debug.LogError("Pixel " + Point.ToString() + "was determined as on edge, but has no empty neighbors");
                    }
                }
            }
        }
        List<EdgePoint> Points = new List<EdgePoint> ();
        EdgePoint RPoint = new EdgePoint(Point, dir);
        EdgePoint LPoint = new EdgePoint(Point, dir);
        Points.Add (LPoint.Copy());
        for (int i = 0; i < Length / 2; i++) {
            if (Renderer.GetPixel(LPoint.Point.x - LPoint.Dir.y + LPoint.Dir.x, LPoint.Point.y + LPoint.Dir.y + LPoint.Dir.x).Active) {
                LPoint = new EdgePoint(new Vector2Int(LPoint.Point.x - LPoint.Dir.y + LPoint.Dir.x, LPoint.Point.y + LPoint.Dir.y + LPoint.Dir.x), new Vector2Int(LPoint.Dir.y, -LPoint.Dir.x));
            }
            else if (Renderer.GetPixel(LPoint.Point.x - LPoint.Dir.y, LPoint.Point.y + LPoint.Dir.x).Active) {
                LPoint = new EdgePoint(new Vector2Int(LPoint.Point.x - LPoint.Dir.y, LPoint.Point.y + LPoint.Dir.x), LPoint.Dir);
            }
            else {
                LPoint = new EdgePoint(LPoint.Point, new Vector2Int(-LPoint.Dir.y, LPoint.Dir.x));
            }
            Points.Add(LPoint.Copy());
            if (RPoint.Point == LPoint.Point && RPoint.Dir == LPoint.Dir)
                break;
        }
        Points.Reverse();
        while (Points.Count<Length) {
            if (RPoint.Point == LPoint.Point && RPoint.Dir == LPoint.Dir)
                break;
            if (Renderer.GetPixel(RPoint.Point.x + RPoint.Dir.y + RPoint.Dir.x, RPoint.Point.y + RPoint.Dir.y - RPoint.Dir.x).Active) {
                RPoint = new EdgePoint(new Vector2Int(RPoint.Point.x + RPoint.Dir.y + RPoint.Dir.x, RPoint.Point.y + RPoint.Dir.y - RPoint.Dir.x), new Vector2Int(-RPoint.Dir.y, RPoint.Dir.x));
            }
            else if (Renderer.GetPixel(RPoint.Point.x + RPoint.Dir.y, RPoint.Point.y - RPoint.Dir.x).Active) {
                RPoint = new EdgePoint(new Vector2Int(RPoint.Point.x + RPoint.Dir.y, RPoint.Point.y - RPoint.Dir.x), RPoint.Dir);
            }
            else {
                RPoint = new EdgePoint(RPoint.Point, new Vector2Int(RPoint.Dir.y, -RPoint.Dir.x));
            }
            Points.Add(RPoint.Copy());
        }
        return Points;
    }
}
