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
    [HideInInspector]
    public bool rendered = false;

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

    public Tuple<PixelState, bool> BuildBiome(Biome biome)
    {
        if (biome.Floor.type != NoiseLayer.NoiseType.Linear) {
            throw new FormatException("Biome floor must be a Linear function");
        }
        if (Parent != null) {
            Vector2 Center = new Vector2(Root.Size / 2, Root.Size / 2);
		    if (Size == 1) {
                Vector2 Rad = Pos - Center;
                //Debug.Log(Rad.magnitude);
			    if (biome.get(Mathf.Atan2(Rad.y, Rad.x)*Rad.magnitude, Rad.magnitude) > 0) {
				    return new Tuple<PixelState, bool>(new PixelState(0, true), true);
			    } else {
                    return new Tuple<PixelState, bool>(new PixelState(0, false), true);
			    }
		    }
        
            //Axises never intersect the current square, so the farest point is one of the angles
            float maxR = 0, minR = Root.Size;
            foreach (Vector2 Delta in Data.Main.Shifts01) {
                maxR = Mathf.Max(maxR, (Center - ((Vector2)Pos - Delta * (Size / 2f))).sqrMagnitude);
                minR = Mathf.Min(minR, (Center - (Pos - Delta * (Size / 2f))).sqrMagnitude);
            }
            maxR = Mathf.Sqrt(maxR);
            minR = Mathf.Sqrt(minR);
            Debug.Log(maxR + " " + minR);
            if (biome.Floor.get(0, minR)+biome.Amplitude < 0) {
                Debug.Log("del"+ biome.Floor.get(0, minR));
                return new Tuple<PixelState, bool>(new PixelState(0, false), true);
            }
            if (biome.Floor.get(0, maxR)-biome.Amplitude > 0) {
                Debug.Log("fill"+ biome.Floor.get(0, maxR));
                return new Tuple<PixelState, bool>(new PixelState(0, true), true);
            }
        }
        if (Color != null)
            InitChildren(Color);
        Color = null;
		Tuple<PixelState, bool>[] Returned = { null, null, null, null };
        bool ok = true;
		for (int i = 0; i < 4; i++) {
			Returned[i] = Children[i].BuildBiome(biome);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
		}
		if (ok) {
            if (Returned[0].Item1 != null)
                return new Tuple<PixelState, bool>(Returned[0].Item1, true);
            return new Tuple<PixelState, bool>(Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
			if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);
		}
		return new Tuple<PixelState, bool>(Color, false);
	}
	public void InitChildren(PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return;
        }
        for (int i = 0; i < Children.Length; i++) {
            Children[i] = new Tree(this, Pos + Data.Main.Shifts01[i] * (Size / 2));
        }
    }
    public Tree Locate(Vector2 Point) {
        if (Color != null)
            return this;
        if (Point.x >= Pos.x + Size / 2) {
            if (Point.y >= Pos.y + Size / 2) {
                return Children[3].Locate(Point);
            }
            else {
                return Children[0].Locate(Point);
            }
        }
        else {
            if (Point.y >= Pos.y + Size / 2) {
                return Children[2].Locate(Point);
            }
            else {
                return Children[1].Locate(Point);
            }
        }
    }
    
    
    
    public Tree LocateUp(Vector2 Point)
    {
        if (Utils.PointInSquare(Pos, Size, Point)) {
            return Locate(Point);
        }
        else return Parent.LocateUp(Point);
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

    public Tuple<PixelState, bool> CircleFill(Vector2 Center, float R, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return null;
        }
        if (NewColor==Color)
            return new Tuple<PixelState, bool>(Color, false);
        if(Size == 1) {
            if(Utils.Distance(Pos+Vector2.one*0.5f, Center) > R) {
                return new Tuple<PixelState, bool>(Color, false);
            } else {
                return new Tuple<PixelState, bool>(NewColor, true);
            }
        }
        if (Utils.Distance(Pos + new Vector2(0, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(0, Size), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, Size), Center) <= R) {
			return new Tuple<PixelState, bool>(NewColor, true);
		}
        if (Center.x >= Pos.x && Center.x <= Pos.x + Size) {
            if (Center.y <= Pos.y - R || Center.y >= Pos.y + Size + R) {
				return new Tuple<PixelState, bool>(Color, false);
			}
        }
        else if (Center.y >= Pos.y && Center.y <= Pos.y + Size) {
            if (Center.x <= Pos.x - R || Center.x >= Pos.x + Size + R) {
				return new Tuple<PixelState, bool>(Color, false);
			}
        }
        else {
            if (Utils.Distance(Pos + new Vector2(0, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(0, Size), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, Size), Center) > R) {
				return new Tuple<PixelState, bool>(Color, false);
			}
        }
        if(Color!=null)
            InitChildren(Color);
        Color = null;
		Tuple<PixelState, bool>[] Returned = { null, null, null, null };
        bool ok = true;
        for (int i = 0; i<4; i++) {
            Returned[i] = Children[i].CircleFill(Center, R, NewColor);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
        }
        if (ok) {
            if(Returned[0].Item1!=null)
			    return new Tuple<PixelState, bool>(Returned[0].Item1, true);
            return new Tuple<PixelState, bool>(Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
			if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);
		}
		return new Tuple<PixelState, bool>(null, false);
	}
    public Tuple<PixelState, bool> TriangleFill(Vector2 A, Vector2 B, Vector2 C, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return null;
        }
        if (NewColor == Color)
			return new Tuple<PixelState, bool>(Color, false);
		if (Size == 1) {
            if (!Utils.PointInTriangle(A, B, C, (Vector2)Pos + Vector2.one * 0.5f)) {
				return new Tuple<PixelState, bool>(Color, false);
			}
            else {
				return new Tuple<PixelState, bool>(NewColor, true);
			}
        }
        if (Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, Size)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, Size))) {
			return new Tuple<PixelState, bool>(NewColor, true);
		}
        if (!Utils.SegmentCrossesSquare(A, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(C, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(A, C, Pos, Size)) {
			return new Tuple<PixelState, bool>(Color, false);
		}
        if (Color != null)
            InitChildren(Color);
        Color = null;
		Tuple<PixelState, bool>[] Returned = { null, null, null, null };
        bool ok = true;
        for (int i = 0; i < 4; i++) {
            Returned[i] = Children[i].TriangleFill(A, B, C, NewColor);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
        }
        if (ok) {  
            if (Returned[0].Item1 != null)
                return new Tuple<PixelState, bool>(Returned[0].Item1, true);
            return new Tuple<PixelState, bool>(Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
            if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);
		}
		return new Tuple<PixelState, bool>(null, false);
	}
}

