using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NoiseLayer
{
	[System.Serializable]
    public class InitList
	{
        public float Frequency = 0;
        public float Amplitude = 0;
		public Vector2 Shift;
	}
    public enum NoiseType {
        Linear,
        Perlin
    }
    public float MinFrequency;
    public float MaxFrequency;
    public float MinAmplitude;
    public float MaxAmplitude;
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
	public float get(float x, float y)
	{
		float x1 = x - Instance.Shift.x;
		float y1 = y - Instance.Shift.y;
		if (type == NoiseType.Linear) {
			return y1 * Instance.Amplitude;
		} else if (type == NoiseType.Perlin) {
			x1 *= Instance.Frequency;
			y1 *= Instance.Frequency;
			return Mathf.PerlinNoise((float)x1, (float)y1)*Instance.Amplitude;
		}
		return 0;
	}
}
