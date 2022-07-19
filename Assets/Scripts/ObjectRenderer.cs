using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRenderer : MonoBehaviour
{
    public Object Parent;
    public List<GameObject> Tracking;
    public int VisionRange;
    public int CellSize;// must be a big enough power of 2, more than VisionRange*2
    public List<RenderArea> Cells = new List<RenderArea>();
    public class RenderArea {
        public Texture2D Tex;
        public PixelState[,] RealTex;
        public Vector2Int Pos;
        public SpriteRenderer Instance;
        public int Size;
        public RenderArea(Vector2Int Pos, int Size, ObjectRenderer Parent) {
            this.Pos = Pos;
            this.Size = Size;
            Tex = new Texture2D(Size, Size, TextureFormat.RGBA32, false);
            RealTex = new PixelState[Size, Size];
            Color[] temp = new Color[Size*Size];
            for (int i = 0; i < Size * Size; i++) {
                temp[i] = new Color(255, 255, 255, 0);
                RealTex[i % Size, i / Size] = new PixelState(0, false);
            }
            Tex.SetPixels(temp);
            Instance = Instantiate(Data.Main.ObjectRenderer).GetComponent<SpriteRenderer>();
            Instance.transform.position = Utils.InverseTransformPos(Pos+new Vector2Int(Size/2, Size/2), Parent.transform, Parent.Parent.Size);
            Instance.sprite = Sprite.Create(Tex, new Rect(0.0f, 0.0f, Tex.width, Tex.height), new Vector2(0.5f, 0.5f));
        }
        public void Draw(Tree Cur) {
            float TStart = Time.realtimeSinceStartup;
            float ttex = 0;
            float tchange = 0;
            Tuple<Vector2, Vector2> Box = Utils.SquaresIntersect(Cur.Pos, Cur.Size, Pos, Size);
            if (Box == null) return;
            if (Cur.Color != null) {
                
                for (int i = (int)Math.Round(Box.Item1.x); i < (int)Math.Round(Box.Item2.x); i++) {
                    for (int j = (int)Math.Round(Box.Item1.y); j < (int)Math.Round(Box.Item2.y); j++) {
                        Color color = Data.Main.Textures[Cur.Color.TextureID].GetPixel(i % Data.Main.Textures[Cur.Color.TextureID].width, j % Data.Main.Textures[Cur.Color.TextureID].height);
                        float t1 = Time.realtimeSinceStartup;
                        if (!Cur.Color.Active)
                            color.a = 0;
                        RealTex[i - Pos.x, j - Pos.y] = Cur.Color;
                        t1 = Time.realtimeSinceStartup;
                        Tex.SetPixel(i - Pos.x, j - Pos.y, color);
                        tchange += Time.realtimeSinceStartup - t1;
                    }
                }
            }
            else {
                foreach (Tree Child in Cur.Children) {
                    Draw(Child);
                }
            }
            Tex.Apply();
            if (TStart > 5)
                Debug.Log("Time on texture change: "+ttex.ToString()+" Time on array change: "+tchange);
        }
    }
    private void Update() {
        foreach (GameObject g in Tracking) {
            for (int dx = -VisionRange; dx < VisionRange + CellSize; dx += CellSize) {
                for (int dy = -VisionRange; dy < VisionRange + CellSize; dy += CellSize) {
                    Vector2 d = new Vector2(dx, dy);
                    Vector2 Point = Utils.TransformPos((Vector2)g.transform.position, Parent.transform, Parent.Size) + d;
                    bool ok = false;
                    foreach (RenderArea Square in Cells) {
                        if (Utils.PointInSquare(Square.Pos, Square.Size, Point)) {
                            ok = true;
                            break;
                        }
                    }
                    if (!ok) {
                        Vector2Int ToSpawn = new Vector2Int(((int)Point.x / CellSize) * CellSize, ((int)Point.y / CellSize) * CellSize);
                        Cells.Add(new RenderArea(ToSpawn, CellSize, this));
                        Cells[Cells.Count - 1].Draw(Parent.Root);

                    }
                }
            }
        }
    }
    public PixelState GetPixel(int x, int y) {
        Vector2 Pos = new Vector2(x, y);
        foreach (RenderArea Cell in Cells) {
            if (Utils.PointInSquare(Cell.Pos, Cell.Size, Pos)) {
                return Cell.RealTex[x-Cell.Pos.x, y-Cell.Pos.y];
            }
        }
        return Parent.Root.Locate(Pos).Color;
    }
    public PixelState GetPixel(Vector2Int Pos) {
        foreach (RenderArea Cell in Cells) {
            if (Utils.PointInSquare(Cell.Pos, Cell.Size, Pos)) {
                return Cell.RealTex[Pos.x - Cell.Pos.x, Pos.y - Cell.Pos.y];
            }
        }
        return Parent.Root.Locate(Pos).Color;
    }
    public void Draw(Tree Square) {
        foreach (RenderArea Cell in Cells) {
            Cell.Draw(Square);
        }
    }
}
