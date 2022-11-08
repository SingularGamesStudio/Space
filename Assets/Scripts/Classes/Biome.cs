using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
[System.Serializable]
public class Biome
{
	public string name;
	public NoiseLayer Floor;
	public List<NoiseLayer> Base;
	public float Amplitude = 0;
	public Biome(Biome toCopy)
	{ // Copy constructor
		name = toCopy.name;
		Amplitude = toCopy.Amplitude;
		Floor = new NoiseLayer(toCopy.Floor);
		Base = new List<NoiseLayer>();
		foreach (NoiseLayer nl in toCopy.Base) {
			Base.Add(new NoiseLayer(nl));
		}
	}
	public void Init(int seed)
	{
		System.Random rnd = new System.Random(seed);
		int seed0 = rnd.Next(1000000);
		Floor.Init(seed0);
		Amplitude = 0;
		foreach (NoiseLayer n in Base) {
			seed0 = rnd.Next(1000000);
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