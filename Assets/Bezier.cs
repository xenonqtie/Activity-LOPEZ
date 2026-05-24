using UnityEngine;

public static class Bezier
{
    // Quadratic Bézier Formula (p0 = start, p1 = control, p2 = end)
    public static Vector3 GetQuadraticPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * p0 + 2f * oneMinusT * t * p1 + t * t * p2;
    }

    // Cubic Bézier Formula (p0 = start, p1 = control1, p2 = control2, p3 = end)
    public static Vector3 GetCubicPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 + 
               3f * oneMinusT * oneMinusT * t * p1 + 
               3f * oneMinusT * t * t * p2 + 
               t * t * t * p3;
    }
}