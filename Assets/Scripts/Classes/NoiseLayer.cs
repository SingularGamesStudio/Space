using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class NoiseLayer
{
    int cnt = 0;
    enum NoiseType {
        Constant,
        Perlin
    }
    List<float> freq = new List<float>();
    List<float> amp = new List<float>();
}
