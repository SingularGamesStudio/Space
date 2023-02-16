using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
[System.Serializable]
public class Biome
{
	public string name;
	public int MinSize;
    public int MaxSize;
	[HideInInspector]
	public int Size;
	[HideInInspector]
	public int LeftEdge;
    [SerializeReference]
    public Func Base;
	public float Amplitude = 0;
	public Biome(Biome toCopy)
	{ // Copy constructor
		name = toCopy.name;
		Amplitude = toCopy.Amplitude;
        MinSize = toCopy.MinSize;
        MaxSize = toCopy.MaxSize;
        Size = toCopy.Size;
		Base = toCopy.Base.DeepCopy();
	}
	public void Init(int seed, int newSize, int PlanetRadius)
	{
		System.Random rnd = new System.Random(seed);
		int seed0 = rnd.Next(1000000);
        Base.Init(seed0);
        /*if (!Floor.isLinear()) {
            throw new System.Exception("Biome does not support non-linear floor");
			//TODO
		}*/
        Amplitude = 0;
        Size = newSize;
	}
    
	public float get(FuncPassType data)
	{
		return Base.get(data);
    }
}