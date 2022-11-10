using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
[System.Serializable]
public class Biome
{
	public string name;
	public NoiseProduct Floor;
	public int MinSize;
    public int MaxSize;
	[HideInInspector]
	public int Size;
	[HideInInspector]
	public int LeftEdge;
    public List<NoiseProduct> Base;
	public float Amplitude = 0;
	public Biome(Biome toCopy)
	{ // Copy constructor
		name = toCopy.name;
		Amplitude = toCopy.Amplitude;
		Floor = new NoiseProduct(toCopy.Floor);
        MinSize = toCopy.MinSize;
        MaxSize = toCopy.MaxSize;
        Size = toCopy.Size;
        Base = new List<NoiseProduct>();
		foreach (NoiseProduct nl in toCopy.Base) {
			Base.Add(new NoiseProduct(nl));
		}
	}
	public void Init(int seed, int newSize, int PlanetRadius)
	{
		System.Random rnd = new System.Random(seed);
		int seed0 = rnd.Next(1000000);
        Floor.Shift(PlanetRadius);
        Floor.Init(seed0);
        if (!Floor.isLinear()) {
            throw new System.Exception("Biome does not support non-linear floor");
        }
        Amplitude = 0;
        Size = newSize;
        foreach (NoiseProduct n in Base) {
			seed0 = rnd.Next(1000000);
			n.Shift(PlanetRadius);
			n.Init(seed0);
			Amplitude += Mathf.Abs(n.getAmplitude());
		}	
	}
    
	public float get(float x, float y)
	{
		float val = Floor.get(x, y);
		foreach (NoiseProduct layer in Base) {
			val += layer.get(x, y);
		}
		return val;
	}
}