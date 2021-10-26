﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scene5;

public class Scene5_PathFinder : MonoBehaviour
{
    public static Scene5_PathFinder Instance;

    List<Path> paths = new List<Path>();
    Dictionary<Scene5_Vertex, List<Scene5_Line>> connectLines = new Dictionary<Scene5_Vertex, List<Scene5_Line>>();
    Scene5_Vertex start;
    Scene5_Vertex end;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        start = Scene5_DrawController.Instance.start;
        end = Scene5_DrawController.Instance.end;
    }

    public void LineCreated(Scene5_Line line)
    {
        AddLineTo(line.start, line);
        AddLineTo(line.end, line);
    }

    void AddLineTo(Scene5_Vertex ver, Scene5_Line line)
    {
        if (!connectLines.ContainsKey(ver))
        {
            connectLines.Add(ver, new List<Scene5_Line>());
        }

        if (!connectLines[ver].Contains(line))
        {
            connectLines[ver].Add(line);
        }
    }

    public void FindShortestPath()
    {
        Dictionary<Scene5_Vertex, float> distance = new Dictionary<Scene5_Vertex, float>();
        List<Scene5_Vertex> greenNodes = new List<Scene5_Vertex>();
        Dictionary<Scene5_Vertex, Scene5_Vertex> parents = new Dictionary<Scene5_Vertex, Scene5_Vertex>();

        foreach (var ver in Scene5_DrawController.Instance.allVers)
        {
            distance.Add(ver, Mathf.Infinity);
        }
        distance[start] = 0f;

        Scene5_Vertex tempVer;

        while (tempVer = GetMinVertex(distance, greenNodes))
        {
            Debug.Log($"tempVer: {tempVer}");
            greenNodes.Add(tempVer);
            foreach (Scene5_Line line in connectLines[tempVer])
            {
                Scene5_Vertex otherVer = GetOtherVer(tempVer, line);
                if (!greenNodes.Contains(otherVer))
                {
                    Debug.Log($"otherVer: {otherVer}");
                    if (distance[tempVer] + line.lenght < distance[otherVer])
                    {
                        parents[otherVer] = tempVer;
                        distance[otherVer] = distance[tempVer] + line.lenght;
                    }
                    Debug.Log($"distance[otherVer]: {distance[otherVer]}");
                }
            }
        }

        Debug.Log($"Result: {distance[end]}");
    }


    Scene5_Vertex GetOtherVer(Scene5_Vertex ver, Scene5_Line line)
    {
        return line.start == ver ? line.end : line.start;
    }

    Scene5_Vertex GetMinVertex(Dictionary<Scene5_Vertex, float> distances, List<Scene5_Vertex> greenNodes)
    {
        float min = Mathf.Infinity;
        Scene5_Vertex ver = null;
        foreach (KeyValuePair<Scene5_Vertex, float> pair in distances)
        {
            if (distances[pair.Key] < min && !greenNodes.Contains(pair.Key))
            {
                ver = pair.Key;
                min = distances[pair.Key];
            }
        }
        return ver;
    }

}

public class Path
{
    public List<Scene5_Line> lines = new List<Scene5_Line>();
    public float lenght = 0f;
}