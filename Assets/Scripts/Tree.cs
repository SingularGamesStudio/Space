using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tree {
    public Tree Parent = null;
    public Object Root = null;
    public Tree[] Children = new Tree[4];//numbered as basic quarters of the plane (right-up, left-up, left-down, right-down)
    public Vector2Int Pos = new Vector2Int(0, 0);
    public int Size = 0;
    public PixelState Color = null;

    public Tree(Tree Parent, Vector2Int Pos) {
        this.Parent = Parent;
        this.Pos = Pos;
        this.Size = Parent.Size / 2;
        this.Root = Parent.Root;
        this.Color = Parent.Color;
    }
    /// <summary>
    /// Initialize root of a tree
    /// </summary>
    public Tree(int Size, Object Root, PixelState Color) {
        this.Parent = null;
        this.Pos = new Vector2Int(0, 0);
        this.Size = Size;
        this.Root = Root;
        this.Color = Color;
    }
    public void InitChildren(PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return;
        }
        for (int i = 0; i < Children.Length; i++) {
            Children[i] = new Tree(this, Pos + Data.Main.Shifts01[i] * (Size / 2));
            //Children[i].Update(NewColor);
        }
    }
    public Tree Locate(Vector2 Point) {
        if (Color != null)
            return this;
        if (Point.x >= Pos.x + Size / 2) {
            if (Point.y >= Pos.x + Size / 2) {
                return Children[3].Locate(Point);
            }
            else {
                return Children[0].Locate(Point);
            }
        }
        else {
            if (Point.y >= Pos.x + Size / 2) {
                return Children[2].Locate(Point);
            }
            else {
                return Children[1].Locate(Point);
            }
        }
    }

    public void Update(PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return;
        }
        Color = NewColor;
        Children = new Tree[4];
        Root.Renderer.Draw(this);
    }

    public PixelState CircleFill(Vector2 Center, float R, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return null;
        }
        if (NewColor==Color)
            return Color;
        if(Size == 1) {
            if(Utils.Distance(Pos+Vector2.one*0.5f, Center) > R) {
                return Color;
            } else {
                Update(NewColor);
                return Color;
            }
        }
        if (Utils.Distance(Pos + new Vector2(0, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(0, Size), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, Size), Center) <= R) {
            Update(NewColor);
            return Color;
        }
        if (Center.x >= Pos.x && Center.x <= Pos.x + Size) {
            if (Center.y <= Pos.y - R || Center.y >= Pos.y + Size + R) {
                return Color;
            }
        }
        else if (Center.y >= Pos.y && Center.y <= Pos.y + Size) {
            if (Center.x <= Pos.x - R || Center.x >= Pos.x + Size + R) {
                return Color;
            }
        }
        else {
            if (Utils.Distance(Pos + new Vector2(0, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(0, Size), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, Size), Center) > R) {
                return Color;
            }
        }
        if(Color!=null)
            InitChildren(NewColor);
        Color = null;
        PixelState[] Returned = { null, null, null, null };
        for (int i = 0; i<4; i++) {
            Returned[i] = Children[i].CircleFill(Center, R, NewColor);
            if (Returned[i] != Returned[0] || Returned[i] == null)
                Returned[0] = null;
        }
        if (Returned[0] != null) {
            Update(NewColor);
        }
        return Color;
    }
    public PixelState TriangleFill(Vector2 A, Vector2 B, Vector2 C, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return null;
        }
        if (NewColor == Color)
            return Color;
        if (Size == 1) {
            if (!Utils.PointInTriangle(A, B, C, Pos + Vector2.one * 0.5f)) {
                return Color;
            }
            else {
                Update(NewColor);
                return Color;
            }
        }
        if (Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, Size)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, Size))) {
            Update(NewColor);
            return Color;
        }
        if (!Utils.SegmentCrossesSquare(A, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(C, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(A, C, Pos, Size)) {
            return Color;
        }
        if (Color != null)
            InitChildren(NewColor);
        Color = null;
        PixelState[] Returned = { null, null, null, null };
        for (int i = 0; i < 4; i++) {
            Returned[i] = Children[i].TriangleFill(A, B, C, NewColor);
            if (Returned[i] != Returned[0] || Returned[i] == null)
                Returned[0] = null;
        }
        if (Returned[0] != null) {
            Update(NewColor);
        }
        return Color;
    }
}

