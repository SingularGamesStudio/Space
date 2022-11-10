using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
[System.Serializable]
public class Biome
{
	public string name;
	public NoiseLayer Floor;
	public int MinSize;
    public int MaxSize;
	public int Size;
    public List<NoiseLayer> Base;
	public float Amplitude = 0;
	public Biome(Biome toCopy)
	{ // Copy constructor
		name = toCopy.name;
		Amplitude = toCopy.Amplitude;
		Floor = new NoiseLayer(toCopy.Floor);
        MinSize = toCopy.MinSize;
        MaxSize = toCopy.MaxSize;
        Size = toCopy.Size;
        Base = new List<NoiseLayer>();
		foreach (NoiseLayer nl in toCopy.Base) {
			Base.Add(new NoiseLayer(nl));
		}
	}
	public void Init(int seed, int newSize, int PlanetRadius)
	{
		System.Random rnd = new System.Random(seed);
		int seed0 = rnd.Next(1000000);
        Floor.Shift(PlanetRadius);
        Floor.Init(seed0);
		Amplitude = 0;
        Size = newSize;
        foreach (NoiseLayer n in Base) {
			seed0 = rnd.Next(1000000);
			n.Shift(PlanetRadius);
			n.Init(seed0);
			Amplitude += Mathf.Abs(n.Instance.Amplitude);
		}	
	}
    
	public float get(float x, float y)
	{
		float val = Floor.get(x, y);
		foreach (NoiseLayer layer in Base) {
			val += layer.get(x, y);
		}
		return val;
	}
}