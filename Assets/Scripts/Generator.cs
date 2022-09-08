using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Generator
{
	
    public static List<Vector2> MakePlanet(int Length, int seed, List<Biome> Biomes)
	{
		System.Random rnd = new System.Random(seed);
		if(Biomes.Count == 1) {
			return MakeBiome(Length, rnd.Next(100000), Biomes[0]);
		}
	}
	public static List<Vector2> MakeBiome(int length, int seed, Biome cur)
	{
		System.Random rnd = new System.Random(seed);
	}
}
