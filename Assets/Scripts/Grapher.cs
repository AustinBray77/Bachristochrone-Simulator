using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

//Class to generate the data graphs
public class Grapher : MonoBehaviour
{
    //Instance variables
    [SerializeField] private float scalingFactor;
    [SerializeField] private Transform canvas, pointContainer, labelContainer;
    [SerializeField] private GameObject point, line, label;
    [SerializeField] private TextMeshProUGUI bestTimes, worstTimes;

    //Method called on scene start
    private void Awake()
    {
        //Loads the data from the file and graphs it
        LoadDataFromFile();
    }

    //Method to load and graph the data from the data file
    public void LoadDataFromFile()
    {
        //Try-catch to prevent file IO errors
        try
        {
            //Gets all lines from data file and initializes a data list
            string[] lines = File.ReadAllLines("output.txt");
            List<DataPoint<DateTime, float>> data = new List<DataPoint<DateTime, float>>();

            //Converts each line into a datapoint and adds it to the data list
            foreach (string line in lines)
            {
                if (line == null || line == "" || line == "\n") continue;

                data.Add(new DataPoint<DateTime, float>(line));
            }

            //Creates the graph points and lines
            DrawTimeLineGraph(data);

            //Creates the graph labels
            CreateLabels(data);

            //Sorts the data using quick sort
            Functions.QSortData<DateTime, float>(data, 0, data.Count);

            //Writes to the UI showing the best and worst times from the data
            bestTimes.text = "Best Times:\n";
            worstTimes.text = "Worst Times:\n";
            for (int i = 0; i < 5; i++)
            {
                bestTimes.text += $"{i + 1}: {data[i].yValue} @ {data[i].xValue}\n";
                worstTimes.text += $"{i + 1}: {data[data.Count - i - 1].yValue} @ {data[data.Count - i - 1].xValue}\n";
            }
        }
        //Log the error is there is one
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    //Method to create the points and lines
    public void DrawTimeLineGraph(List<DataPoint<DateTime, float>> data)
    {
        //If there is no data, return
        if (data == null) return;
        if (data.Count < 1) return;

        //Minimum and maxiumum x values
        DateTime minX = data[0].xValue,
            maxX = data[data.Count - 1].xValue;

        //Calculates the minimum and maximum y values and removes the duplicates
        float minY = float.MaxValue, maxY = float.MinValue;
        HashSet<DataPoint<DateTime, float>> set = new HashSet<DataPoint<DateTime, float>>();
        foreach (DataPoint<DateTime, float> point in data)
        {
            //If the point already exists, continue
            if (set.Contains(point)) continue;

            //If the point is less than the current minimum value, set it to the minimum value
            if (point.yValue < minY)
            {
                minY = point.yValue;
            }

            //If the point is less than the current minimum value, set it to the maximum value
            if (point.yValue > maxY)
            {
                maxY = point.yValue;
            }

            //Add the point to the hashset
            set.Add(point);
        }

        //Array to store the instantiated points
        GameObject[] points = new GameObject[set.Count];

        //Sets the data to data with removed duplicates
        data = set.ToList();

        //Loops to create each point
        for (int i = 0; i < points.Length; i++)
        {
            //Instantiates the point into the scene
            points[i] = Instantiate(point, pointContainer);

            //Calculates the x and y values of the point
            float x = (float)((data[i].xValue - minX).TotalSeconds / (maxX - minX).TotalSeconds) * scalingFactor * (1920f / 1080f) * canvas.transform.localScale.x,
                y = (1080f / 1920f) * canvas.transform.localScale.y * scalingFactor * (data[i].yValue - minY) / (maxY - minY);

            //Sets the position to the calculated values
            points[i].transform.localPosition = new Vector3(x, y);

            //If not on the first point, draw a line between this point and the last point
            if (i > 0)
            {
                //Instantiates the line
                GameObject lineObj = Instantiate(line, pointContainer);

                //Sets the position to the average position of the two points
                lineObj.transform.localPosition = (points[i].transform.localPosition + points[i - 1].transform.localPosition) / 2;

                //Scales the line to reach both points
                lineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(10, Vector3.Distance(points[i].transform.localPosition, points[i - 1].transform.localPosition));

                //Calculates the difference in the x and y positions
                float xDiff = points[i].transform.localPosition.x - points[i - 1].transform.localPosition.x,
                    yDiff = points[i].transform.localPosition.y - points[i - 1].transform.localPosition.y;

                //Uses the x and y differences to calculate the angle of the line
                lineObj.transform.rotation = Quaternion.Euler(0, 0, yDiff != 0 ? Mathf.Rad2Deg * -Mathf.Atan(xDiff / yDiff) : 0);
            }
        }
    }

    //Method to create the labels for the data
    public void CreateLabels(List<DataPoint<DateTime, float>> data)
    {
        //Minimum and maxiumum x values   
        DateTime minX = data[0].xValue,
            maxX = data[data.Count - 1].xValue;

        //Calculates the minimum and maximum y values
        float minY = float.MaxValue, maxY = float.MinValue;
        foreach (DataPoint<DateTime, float> point in data)
        {
            //If the point is less than the current minimum value, set it to the minimum value
            if (point.yValue < minY)
            {
                minY = point.yValue;
            }

            //If the point is less than the current minimum value, set it to the maximum value
            if (point.yValue > maxY)
            {
                maxY = point.yValue;
            }
        }

        //Creates X labels
        for (int i = 0; i < 4; i++)
        {
            //Instantiates the label and gets the text component
            TextMeshProUGUI labelObj = Instantiate(label, labelContainer).GetComponent<TextMeshProUGUI>();

            //Sets the position and text
            labelObj.transform.localPosition = new Vector2(i * (scalingFactor / 3f) * 1920 / 1080 * canvas.transform.localScale.x, -50);
            labelObj.text = (minX.Add(TimeSpan.FromSeconds((maxX - minX).TotalSeconds * (i / (float)3)))).ToString();
        }

        //Creates Y labels
        for (int i = 0; i < 4; i++)
        {
            //Instantiates the label and gets the text component
            TextMeshProUGUI labelObj = Instantiate(label, labelContainer).GetComponent<TextMeshProUGUI>();

            //Sets the position and text
            labelObj.transform.localPosition = new Vector2(-25, i * (scalingFactor / 3f) * 1080 / 1920 * canvas.transform.localScale.y);
            labelObj.text = Functions.RoundToDecimalPlaces(((maxY - minY) * (i / (float)3)) + minY, 2).ToString();
        }
    }
}
