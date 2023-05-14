using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Bbezier curve algorithm
public class SplineGenerator : MonoBehaviour
{
    public int segments = 10;
    public float lineWidth = 0.1f;
    public Color lineColor = Color.white;

    private Vector2[] controlPoints;
    private LineRenderer lineRenderer;

    void Start()
    {
        // defining a list of control points (x and y coordinates)
        //using a list of x and y coordinates to implement
        float[] xPoints = new float[] { 0, 1, 2, 3 };
        float[] yPoints = new float[] { 0, 2, 1, 3 };
        controlPoints = new Vector2[xPoints.Length];

        for (int i = 0; i < xPoints.Length; i++)
        {
            controlPoints[i] = new Vector2(xPoints[i], yPoints[i]);
        }

        // generate the spline curve
        Vector3[] splinePoints = GenerateSpline(controlPoints.ToList(), segments);

        // set up the line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = lineColor;
        lineRenderer.positionCount = splinePoints.Length;

        // set the positions of the line renderer
        for (int i = 0; i < splinePoints.Length; i++)
        {
            lineRenderer.SetPosition(i, splinePoints[i]);
        }
    }

    private Vector3[] GenerateSpline(List<Vector2> controlPoints, int segments)
    {
        Vector3[] splinePoints = new Vector3[segments];

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (float)(segments - 1);
            splinePoints[i] = CalculateBezierPoint(t, controlPoints);
        }

        return splinePoints;
    }


    private Vector3 CalculateBezierPoint(float t, List<Vector2> controlPoints)
    {
        int n = controlPoints.Count - 1;
        Vector3 point = Vector3.zero;

        for (int i = 0; i <= n; i++)
        {
            float b = BinomialCoefficient(n, i);
            float power1 = Mathf.Pow(1 - t, n - i);
            float power2 = Mathf.Pow(t, i);
            Vector2 controlPoint = controlPoints[i];
            Vector3 point2D = new Vector3(controlPoint.x, controlPoint.y, 0);

            point += b * power1 * power2 * point2D;
        }

        return point;
    }

    private float BinomialCoefficient(int n, int k)
    {
        float result = 1;

        for (int i = 1; i <= k; i++)
        {
            result *= (float)(n - (k - i)) / i;
        }

        return result;
    }
}
