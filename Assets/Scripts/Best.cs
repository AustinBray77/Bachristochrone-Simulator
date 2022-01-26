using System;
using System.IO;
using System.Collections;
using UnityEngine;

//Class to find the best AI result from best.txt
public class Best : MonoBehaviour
{
    //Refrences to objects in the scene
    [SerializeField] private Ball straightestPath, bestAI;
    [SerializeField] private Generator straightestGen, bestGen;
    [SerializeField] private Transform straightestEnd;

    //Method called on scene start
    private IEnumerator Start()
    {
        //Wait until the gen points have been initialized then load the best points
        yield return new WaitUntil(() => bestGen.GetPoints() != null);
        LoadBestPoints();
    }

    //Method called to load the best points from best.txt
    private void LoadBestPoints()
    {
        //Try-Catch to catch any file IO errors
        try
        {
            //Reads all of the lines from best.txt
            string[] lines = File.ReadAllLines("best.txt");

            //Initializes variables for finding the best time
            float bestTime = float.MaxValue;
            int bestIndex = 0;

            //Loops through every other line
            for (int i = 0; i < lines.Length; i += 2)
            {
                //Skip lines that are empty
                while (lines[i] == "\n" || lines[i] == "" || String.IsNullOrWhiteSpace(lines[i]))
                {
                    i++;
                }

                //Parse the current line
                float.TryParse(lines[i], out float time);

                //If the current line time is less than the best time, set it as the new best time
                if (time < bestTime)
                {
                    bestTime = time;
                    bestIndex = i;
                }
            }

            //Gets all of the points in the best path
            string[] spacedPoints = lines[bestIndex + 1].Split(' ');

            //Loops through each point
            foreach (string point in spacedPoints)
            {
                //If the point is empty, continue
                if (String.IsNullOrWhiteSpace(point) || point == "" || point == "\n") continue;

                //Splits the point into its x and y compontents
                string[] data = point.Split(',');

                //Parses the components
                float.TryParse(data[0].Substring(1, data[0].Length - 1), out float x);
                float.TryParse(data[1].Substring(0, data[1].Length - 1), out float y);

                //Adds the point to the generator
                bestGen.AddPoint(new Vector3(x, y) + bestGen.transform.position);
            }

            //Adds the points for a straight line to the straight line generator
            straightestGen.AddPoint(straightestPath.transform.position - new Vector3(1, 0));
            straightestGen.AddPoint(straightestEnd.transform.position + new Vector3(1, -1));
        }
        //Log the error if there is one
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
