using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour
{
    public static Data Main;
    public int MaxSpriteSize;
    public List<Texture2D> Textures;
    public List<Biome> Biomes;
    public List<Material> Materials;
    public GameObject PlanetRenderer;
    public GameObject CameraRenderer;
    public GameObject EmptySprite;
    public int MinBiomeSize = 100000;
    public readonly Vector2Int[] Shifts01 = { new Vector2Int(1, 0), new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1) };
    public readonly Vector2Int[] ShiftsLR = { new Vector2Int(1, 0), new Vector2Int(-1, 0), new Vector2Int(0, 1), new Vector2Int(0, -1) };
    private void Awake() {
        Main = this;
        foreach (Biome biome in Biomes) {
            MinBiomeSize = Mathf.Min(MinBiomeSize, biome.MinSize);
        }

    }
}
