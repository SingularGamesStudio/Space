using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Tree {
    //structure
    public Tree Parent = null;
    public Planet Root = null;
	public Tree[] Children = new Tree[4];//numbered as basic quarters of the plane (right-up, left-up, left-down, right-down)
    //shape
	public Vector2Int Pos = new Vector2Int(0, 0);
    public int Size = 0;
    //contents
    public PixelState Color = null;
	[HideInInspector]
    public bool finishedRender = false; 
	[HideInInspector]
	public bool inRenderQueue = false;

    public Tree(Tree Parent, Vector2Int Pos, bool render = true) {
        this.Parent = Parent;
        this.Pos = Pos;
        this.Size = Parent.Size / 2;
        this.Root = Parent.Root;
        this.Color = Parent.Color;
        if(render)
            TryUpdate(Parent.Color);
	}
    /// <summary>
    /// Initialize root of a tree
    /// </summary>
    public Tree(int Size, Planet Root, PixelState Color, bool render = true) {
        this.Parent = null;
        this.Pos = new Vector2Int(0, 0);
        this.Size = Size;
        this.Root = Root;
		this.Color = Color;
		if (render)
			TryUpdate(Color);
	}

    public Rect GetRect()
    {
        return new Rect(Utils.InverseTransformPos(Pos, Root.transform, Root.Size), Utils.InverseTransformPos(Pos + new Vector2(Size, Size), Root.transform, Root.Size) - Utils.InverseTransformPos(Pos, Root.transform, Root.Size));
    }

	/// <summary>
	/// draw biome at this square
	/// </summary>
	/// <param name="biome">biome to draw, or null to use the one from the planet</param>
	/// <returns></returns>
	public ValueTuple<PixelState, bool> BuildBiome(Biome biome = null)
    {
        if (biome == null) {
            Vector2 Rad = Pos - new Vector2(Root.Size / 2, Root.Size / 2);
            if (Size == 1) {
                biome = Root.Biomes[Root.BiomeByPos[(int)((Mathf.Atan2(Rad.y, Rad.x) + Mathf.PI) * Root.PlanetRadius)]];
            }
            else {
                int[] biomes = { -1, -1, -1, -1 };
                bool ok1 = true;
                for(int i = 0; i<4; i++) {
                    Vector2 Rad1 = Rad + Data.Main.Shifts01[i] * Size;
                    biomes[i] = Root.BiomeByPos[(int)((Mathf.Atan2(Rad1.y, Rad1.x) + Mathf.PI) * Root.PlanetRadius)];
                    ok1 = (ok1 && (biomes[i] == biomes[0]));
                }
                if (ok1)
                    biome = Root.Biomes[biomes[0]];
            }
        }
        if (biome!=null && Parent != null) {
            Vector2 Center = new Vector2(Root.Size / 2, Root.Size / 2);
		    if (Size == 1) {
                Vector2 Rad = Pos - Center;
			    if (Root.GetSmoothBiomeValue(new FuncPassType(Rad.x, Rad.y)) > 0) {//TODO: the only recursion exit is getting to size 1, feels bad, no idea how to optimize
				    return  (new PixelState(0, true), true);
			    } else {
                    return  (new PixelState(0, false), true);
			    }
		    }
        }
        if (Color != null)
			InitChildren(false);
		ValueTuple<PixelState, bool>[] Returned = { (null, false), (null, false), (null, false), (null, false) };
        bool ok = true;
		for (int i = 0; i < 4; i++) {
			Returned[i] = Children[i].BuildBiome(biome);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
		}
		if (ok) {
            if (Returned[0].Item1 != null)
                return  (Returned[0].Item1, true);
            return  (Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
			if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);//TODO:Breaks if the whole planet is single-colored
		}
		return  (null, false);
	}

    /// <summary>
    /// creates children in the tree and propagates the color to them
    /// </summary>
	private void InitChildren(bool render = true) {
        if (Color == null) {
            Debug.LogError("Color not defined");
            return;
        }
        for (int i = 0; i < Children.Length; i++) {
            Children[i] = new Tree(this, Pos + Data.Main.Shifts01[i] * (Size / 2), render);
        }
        Color = null;
    }

    /// <summary>
    /// find a leaf containing given point
    /// </summary>
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
    /// <summary>
    /// find closest parent containing a point
    /// </summary>
    public Tree LocateUp(Vector2 Point)
    {
        if (Utils.PointInSquare(Pos, Size, Point)) {
            return Locate(Point);
        }
        else return Parent.LocateUp(Point);
    }
    /// <summary>
    /// Fill square with a color (deletes children, if any)
    /// </summary>
    private void Update(PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return;
        }
		Color = NewColor;
        Children = new Tree[4];
		Root.Renderer.Draw(this);//TODO
    }
	/// <summary>
	/// Fill square with a color (deletes children, if any)
	/// </summary>
	private void TryUpdate(PixelState NewColor)
	{
		if (NewColor == null) {
			return;
		}
		Update(NewColor);
	}

	public ValueTuple<PixelState, bool> CircleFill(Vector2 Center, float R, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return (null, false);
        }
        if (NewColor==Color)
            return  (Color, false);
        if(Size == 1) {
            if(Utils.Distance(Pos+Vector2.one*0.5f, Center) > R) {
                return  (Color, false);
            } else {
                return  (NewColor, true);
            }
        }
        if (Utils.Distance(Pos + new Vector2(0, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(0, Size), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, 0), Center) <= R &&
            Utils.Distance(Pos + new Vector2(Size, Size), Center) <= R) {
			return  (NewColor, true);
		}
        if (Center.x >= Pos.x && Center.x <= Pos.x + Size) {
            if (Center.y <= Pos.y - R || Center.y >= Pos.y + Size + R) {
				return  (Color, false);
			}
        }
        else if (Center.y >= Pos.y && Center.y <= Pos.y + Size) {
            if (Center.x <= Pos.x - R || Center.x >= Pos.x + Size + R) {
				return  (Color, false);
			}
        }
        else {
            if (Utils.Distance(Pos + new Vector2(0, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(0, Size), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, 0), Center) > R &&
                Utils.Distance(Pos + new Vector2(Size, Size), Center) > R) {
				return  (Color, false);
			}
        }
        if(Color!=null)
            InitChildren(false);
		ValueTuple<PixelState, bool>[] Returned = { (null, false), (null, false), (null, false), (null, false) };
        bool ok = true;
        for (int i = 0; i<4; i++) {
            Returned[i] = Children[i].CircleFill(Center, R, NewColor);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
        }
        if (ok) {
            if(Returned[0].Item1!=null)
			    return  (Returned[0].Item1, true);
            return  (Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
			if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);
		}
		return  (null, false);
	}
    public ValueTuple<PixelState, bool> TriangleFill(Vector2 A, Vector2 B, Vector2 C, PixelState NewColor) {
        if (NewColor == null) {
            Debug.LogError("Color not defined");
            return (null, false);
        }
        if (NewColor == Color)
			return  (Color, false);
		if (Size == 1) {
            if (!Utils.PointInTriangle(A, B, C, (Vector2)Pos + Vector2.one * 0.5f)) {
				return  (Color, false);
			}
            else {
				return  (NewColor, true);
			}
        }
        if (Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, 0)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(0, Size)) &&
            Utils.PointInTriangle(A, B, C, Pos + new Vector2(Size, Size))) {
			return  (NewColor, true);
		}
        if (!Utils.SegmentCrossesSquare(A, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(C, B, Pos, Size) &&
            !Utils.SegmentCrossesSquare(A, C, Pos, Size)) {
			return  (Color, false);
		}
        if (Color != null)
            InitChildren(false);
		ValueTuple<PixelState, bool>[] Returned = { (null, false), (null, false), (null, false), (null, false) };
        bool ok = true;
        for (int i = 0; i < 4; i++) {
            Returned[i] = Children[i].TriangleFill(A, B, C, NewColor);
            if (Returned[i].Item1 == null || Returned[i].Item1 != Returned[0].Item1)
                ok = false;
        }
        if (ok) {  
            if (Returned[0].Item1 != null)
                return  (Returned[0].Item1, true);
            return  (Returned[0].Item1, false);
        }
		for (int i = 0; i < 4; i++) {
            if (Returned[i].Item2)
				Children[i].Update(Returned[i].Item1);
		}
		return  (null, false);
	}
}

