using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NoiseProduct
{
    public List<NoiseLayer> Noises = new List<NoiseLayer>();
    public NoiseProduct(NoiseProduct toCopy) {
        foreach (NoiseLayer noise in toCopy.Noises) {
            Noises.Add(new NoiseLayer(noise));
        }
    }
    public void Init(int seed) {
        System.Random rnd = new System.Random(seed);
        foreach (NoiseLayer noise in Noises) {
            noise.Init(rnd.Next(1000000));
        }
    }
    public float get(float x, float y) {
        float val = 1;
        foreach (NoiseLayer noise in Noises) {
            val *= noise.get(x, y);
        }
        return val;
    }
    public float getAmplitude() {
        float val = 1;
        foreach (NoiseLayer noise in Noises) {
            if (noise.type == NoiseLayer.NoiseType.Linear) {
                throw new System.Exception("NoiseProduct.getAmplitude() does not support Linear noise");
            }
            val *= Mathf.Abs(noise.Instance.Amplitude);
        }
        return val;
    }
    public void Shift(int PlanetRadius) {
        foreach (NoiseLayer noise in Noises) {
            noise.Shift(PlanetRadius);
        }
    }
    public bool isLinear() {
        foreach (NoiseLayer noise in Noises) {
            if (noise.type != NoiseLayer.NoiseType.Linear && noise.type != NoiseLayer.NoiseType.Arctg && noise.type != NoiseLayer.NoiseType.Sign) {
                return false;
            }
        }
        return true;
    }
}
    
