using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using System.Linq;

public class Object : MonoBehaviour {
    public Tree Root;
    public int Size = 256;
    public bool DebugMode;
    public ObjectRenderer Renderer;
    public List<Biome> Biomes;
    public int[] BiomeByPos;
    public int PlanetRadius = 128;
    public int seed = 42;

    void Start() {
        Root = new Tree(Size, this, new PixelState(0, true));
        InitPlanet(seed);
        Root.BuildBiome();
    }
    private void Update() {
        if (Input.GetMouseButtonUp(0)) {
            Vector2 Pos = Utils.TransformPos(Camera.main.ScreenToWorldPoint(Input.mousePosition), transform, Size);
            List<EdgePoint> Edge = GetEdge(new Vector2Int((int)Pos.x, (int)Pos.y), 300, 8);
            if (Edge != null) {
                List<float> Curve;
                Curve = Curves.Curve_Square(100, 300);
                //Curve = Curves.Curve_Perlin(25, 100, 300);
                List<Vector2> NewEdge = Curves.Shift(Edge, Curve);
                
                Transform(Edge, NewEdge);
            }
        }
    }

    public void InitPlanet(int seed)
	{
        System.Random rnd = new System.Random(seed);
        int Length = (int)(PlanetRadius * Mathf.PI * 2f);
        BiomeByPos = new int[Length+1];
        int iter = 0;
        while (Length > 0) {
            int id = rnd.Next(Data.Main.Biomes.Count);
            while (Data.Main.Biomes[id].MinSize > Length) {
                id = rnd.Next(Data.Main.Biomes.Count);
            }
            Biome biome = new Biome(Data.Main.Biomes[id]);
            if (Data.Main.MinBiomeSize + biome.MinSize > Length)
                biome.Init(rnd.Next(100000), Length, PlanetRadius);
            else biome.Init(rnd.Next(100000), rnd.Next(biome.MinSize, Mathf.Min(Length-Data.Main.MinBiomeSize, biome.MaxSize)), PlanetRadius);
            biome.LeftEdge = iter;
            Biomes.Add(biome);
            Length -= biome.Size;
            for (int i = iter; i < iter + biome.Size; i++) {
                BiomeByPos[i] = Biomes.Count - 1;
            }
            iter += biome.Size;
            
        }
        BiomeByPos[Length] = Biomes.Count-1;
    }
    public float GetSmoothBiomeValue(FuncPassType data)
    {
        float polarX = (Mathf.Atan2(data.y, data.x) + Mathf.PI) * PlanetRadius;
		int biomeID = BiomeByPos[(int)polarX];
        int delta = ((int)(polarX + Biomes[biomeID].Size / 10f)) % BiomeByPos.Length;
		if (BiomeByPos[delta] != biomeID) {
            //Right edge of biome
            int biome2ID = BiomeByPos[delta];
            float prop = 0.5f+0.5f*(10f*(Biomes[biomeID].LeftEdge + Biomes[biomeID].Size - polarX)) / Biomes[biomeID].Size;
            //TODO: Clamp value after fixing floor
            return prop * Biomes[biomeID].get(new FuncPassType(data.x, data.y, PlanetRadius, Biomes[biomeID].LeftEdge+ 0.5f*Biomes[biomeID].Size)) +
				(1f - prop) * Biomes[biome2ID].get(new FuncPassType(/*TODOdelta - Biomes[biomeID].Size / 10f*/data.x, data.y, PlanetRadius, Biomes[biome2ID].LeftEdge + 0.5f * Biomes[biome2ID].Size));
		}
		delta = ((int)(polarX - Biomes[biomeID].Size / 10f)+ BiomeByPos.Length) % BiomeByPos.Length;
		if (BiomeByPos[delta] != biomeID) {
			//Left edge of biome
			int biome2ID = BiomeByPos[delta];
			float prop = 0.5f + 0.5f * (10f * (polarX - Biomes[biomeID].LeftEdge)) / Biomes[biomeID].Size;
			return prop * Biomes[biomeID].get(new FuncPassType(data.x, data.y, PlanetRadius, Biomes[biomeID].LeftEdge + 0.5f * Biomes[biomeID].Size)) + 
                (1f - prop) * Biomes[biome2ID].get(new FuncPassType(/*TODOdelta+ Biomes[biomeID].Size / 10f*/data.x, data.y, PlanetRadius, Biomes[biome2ID].LeftEdge + 0.5f * Biomes[biome2ID].Size));
		}
        return Biomes[biomeID].get(new FuncPassType(data.x, data.y, PlanetRadius, Biomes[biomeID].LeftEdge + 0.5f * Biomes[biomeID].Size));
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
