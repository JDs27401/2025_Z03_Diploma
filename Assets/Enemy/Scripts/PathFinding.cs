using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class PathFinding : MonoBehaviour
{
    public static List<Vector2> Dumb(Vector2 destignation)
    {
        var path = new List<Vector2>();
        path.Add(destignation);
        return path;
    }
}
