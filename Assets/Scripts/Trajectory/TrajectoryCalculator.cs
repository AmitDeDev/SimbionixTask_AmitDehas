using System.Collections.Generic;
using UnityEngine;

public class TrajectoryCalculator
{
    public static List<Vector3> CalculateTrajectory(Vector3 start, Vector3 velocity, Vector3 gravity, int steps)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < steps; i++)
        {
            float t = i * 0.1f; // Time step
            Vector3 point = start + velocity * t + 0.5f * gravity * t * t;
            points.Add(point);
        }
        return points;
    }
}