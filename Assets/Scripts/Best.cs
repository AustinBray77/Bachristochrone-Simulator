using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class Best : MonoBehaviour
{
    [SerializeField] private Ball straightestPath, bestAI;

    private void Awake()
    {
        List<Vector2> points = LoadBestPoints();
    }

    private List<Vector2> LoadBestPoints()
    {
        List<Vector2> points = new List<Vector2>();

        try
        {
            string[] lines = File.ReadAllLines("best.txt");
            float bestTime = float.MaxValue;
            int bestIndex = 0;

            for (int i = 0; i < lines.Length; i += 2)
            {
                while (lines[i] == "\n" || lines[i] == "" || lines[i] == null)
                {
                    i++;
                }

                float.TryParse(lines[0], out float time);

                if (time < bestTime)
                {
                    bestTime = time;
                    bestIndex = i;
                }
            }

            string[] spacedPoints = lines[bestIndex + 1].Split(' ');

            foreach (string point in spacedPoints)
            {
                string[] data = point.Split(',');
                float.TryParse(data[0].Substring(1, data[0].Length - 1), out float x);
                float.TryParse(data[1].Substring(0, data[1].Length - 1), out float y);

                points.Add(new Vector2(x, y));
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return points;
    }
}
