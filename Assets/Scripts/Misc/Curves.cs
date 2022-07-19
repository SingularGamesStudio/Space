using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Curves
{

    /// <summary>
    /// Shifts points by a given curve
    /// </summary>
    /// <param name="Base"></param>
    /// <param name="Curve">Number of points must be from 2 to number of points in Base</param>
    /// <param name="Interpolate">Number of pixels to take around each one to interpolate its position. If 0, then determined by the size of the curve.</param>
    /// <returns>List of points, same size as the Curve, stretched to the Base</returns>
    public static List<Vector2> Shift(List<EdgePoint> Base, List<float> Curve, int Interpolate = 0) {
        int Accuracy = Curve.Count;
        if(Accuracy<2 || Accuracy > Base.Count) {
            Debug.LogError("Curve size out of bounds");
            return null;
        }
        Vector2 Dir = new Vector2Int(0, 0);
        foreach(EdgePoint p in Base) {
            Dir += p.Dir;
        }
        Dir.Normalize();
        float Delta = ((float)Base.Count-1) / (Accuracy - 1);
        List<Vector2> Points = new List<Vector2>();
        for (int i = 0; i < Accuracy; i++) {
            Vector2 Pos = new Vector2(0, 0);
            int PointsCnt = 0;
            if (i == 0 || i == Accuracy - 1) {
                Pos = Base[(int)(Delta * i)].Point;
                PointsCnt = 1;
            } else {
                if (Interpolate == 0) {
                    for (int j = (int)(Delta * (i - 0.5f)); j <= (int)(Delta * (i + 0.5f)); j++) {
                        Pos += Base[j].Point;
                        PointsCnt++;
                    }
                }
                else {
                    for (int j = (int)(Delta * i)-Interpolate/2; j <= (int)(Delta * i) + Interpolate / 2; j++) {
                        Pos += Base[j].Point;
                        PointsCnt++;
                    }
                }
            }
            Pos /= PointsCnt;
            Points.Add(Pos + Dir * Curve[i]);
        }
        return Points;
    }
    public static List<float> Curve_Square(int Height, int Accuracy) {
        List<float> Res = new List<float>();
        if (Accuracy < 2) {
            Debug.LogError("Accuracy out of bounds");
            return null;
        }
        for (int i = 0; i < Accuracy; i++) {
            Res.Add(Height);
        }
        return Res;
    }
}
