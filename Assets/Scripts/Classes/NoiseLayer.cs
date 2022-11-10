using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NoiseLayer
{
    
    public enum NoiseType {
        COORD,
        Linear,
        Perlin,
        Sign,
        Arctg
    }
    [System.Serializable]
    public class InitList
	{
        public float Frequency = 1;
        public float Amplitude = 0;
		public Vector2 Shift;
	}
    
    public float MinFrequency = 1;
    public float MaxFrequency = 1;
    public float MinAmplitude = 1;
    public float MaxAmplitude = 1;
	public Vector2 MinShift;
	public Vector2 MaxShift;
    public NoiseType type;
    [HideInInspector]
    public InitList Instance;
	public void Init(int seed)
	{
		Instance = new InitList();
		System.Random rnd = new System.Random(seed);
		Instance.Frequency = ((float)rnd.NextDouble()) * (MaxFrequency - MinFrequency) + MinFrequency;
		Instance.Amplitude = ((float)rnd.NextDouble()) * (MaxAmplitude - MinAmplitude) + MinAmplitude;
		Instance.Shift = ((float)rnd.NextDouble()) * (MaxShift - MinShift) + MinShift;
	}
	public NoiseLayer(NoiseLayer toCopy)
	{ // Copy constructor
		MinFrequency = toCopy.MinFrequency;
		MaxFrequency = toCopy.MaxFrequency;
		MinAmplitude = toCopy.MinAmplitude;
		MaxAmplitude = toCopy.MaxAmplitude;
        MinShift = toCopy.MinShift;
        MaxShift = toCopy.MaxShift;
        type = toCopy.type;
		Instance = new InitList();
		Instance.Frequency = toCopy.Instance.Frequency;
		Instance.Amplitude = toCopy.Instance.Amplitude;
        Instance.Shift = toCopy.Instance.Shift;
    }
    public void Shift(int PlanetRadius) {
		MinShift += new Vector2(0, PlanetRadius);
        MaxShift += new Vector2(0, PlanetRadius);
        Instance.Shift += new Vector2(0, PlanetRadius);
    }
	public float get(float x, float y)
	{
		float x1 = x - Instance.Shift.x;
		float y1 = y - Instance.Shift.y;
		switch (type) {
			case NoiseType.Linear:
				return Instance.Amplitude * (y1);
			case NoiseType.Perlin:
				return Instance.Amplitude * (Mathf.PerlinNoise(x1 * Instance.Frequency, y1 * Instance.Frequency)*2f-1f);
			case NoiseType.Sign:
				return Instance.Amplitude * Mathf.Sign(y1);
			case NoiseType.Arctg:
				return Instance.Amplitude * Mathf.Atan(y1 * Instance.Frequency)/Mathf.PI*2f;
			default:
				return 0;
		}
	}
}
