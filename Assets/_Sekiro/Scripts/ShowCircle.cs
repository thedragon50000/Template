using System;
using R3;
using UnityEngine;


public class ShowCircle : MonoBehaviour
{
    public LineRenderer lineRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Observable.Timer(TimeSpan.FromSeconds(2f)).Subscribe(_ =>
        {
            DrawCircle(10, 10);
        });
    }
    public void DrawCircle(float radius, int segments)
    {
        LineRenderer line = GetComponent<LineRenderer>();
        line.startColor = Color.cyan;
        line.endColor = Color.aliceBlue;

        line.positionCount = segments + 1;
        line.loop = true;

        for (int i = 0; i <= segments; i++)
        {
            float angle = (i / (float)segments) * Mathf.PI * 2;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;
            line.SetPosition(i, new Vector3(x, 0, z));
        }
    }
}
