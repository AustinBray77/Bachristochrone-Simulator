using System;
using System.IO;
using System.Collections;
using UnityEngine;

public class Best : MonoBehaviour
{
    [SerializeField] private Ball straightestPath, bestAI;
    [SerializeField] private Generator straightestGen, bestGen;
    [SerializeField] private Transform straightestEnd;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => bestGen.GetPoints() != null);
        LoadBestPoints();
    }

    private void LoadBestPoints()
    {
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

                float.TryParse(lines[i], out float time);

                if (time < bestTime)
                {
                    bestTime = time;
                    bestIndex = i;
                }
            }

            string[] spacedPoints = lines[bestIndex + 1].Split(' ');

            foreach (string point in spacedPoints)
            {
                if (point == null || point == "" || point == "\n") continue;

                string[] data = point.Split(',');
                float.TryParse(data[0].Substring(1, data[0].Length - 1), out float x);
                float.TryParse(data[1].Substring(0, data[1].Length - 1), out float y);

                bestGen.AddPoint(new Vector3(x, y) + bestGen.transform.position);
            }

            straightestGen.AddPoint(straightestPath.transform.position - new Vector3(1, 0));
            straightestGen.AddPoint(straightestEnd.transform.position + new Vector3(1, -1));
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
