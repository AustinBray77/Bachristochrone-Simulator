using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

//Class to generate the data graphs
public class Grapher : MonoBehaviour
{
    [SerializeField] private float scalingFactor;
    [SerializeField] private Transform canvas, pointContainer, labelContainer;
    [SerializeField] private GameObject point, line, label;
    [SerializeField] private TextMeshProUGUI bestTimes, worstTimes;

    private void Awake()
    {
        Debug.Log("Awake");
        LoadDataFromFile();
    }

    public void LoadDataFromFile()
    {
        try
        {
            string[] lines = File.ReadAllLines("output.txt");

            List<DataPoint<DateTime, float>> data = new List<DataPoint<DateTime, float>>();

            foreach (string line in lines)
            {
                if (line == null || line == "" || line == "\n") continue;

                data.Add(new DataPoint<DateTime, float>(line));
            }

            DrawTimeLineGraph(data);
            CreateLabels(data);

            Functions.QSortData<DateTime, float>(data, 0, data.Count);

            bestTimes.text = "Best Times:\n";
            worstTimes.text = "Worst Times:\n";

            for (int i = 0; i < 5; i++)
            {
                bestTimes.text += $"{i + 1}: {data[i].yValue} @ {data[i].xValue}\n";
                worstTimes.text += $"{i + 1}: {data[data.Count - i - 1].yValue} @ {data[data.Count - i - 1].xValue}\n";
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    public void DrawTimeLineGraph(List<DataPoint<DateTime, float>> data)
    {
        if (data == null) return;
        if (data.Count < 1) return;

        DateTime minX = data[0].xValue,
            maxX = data[data.Count - 1].xValue;

        float minY = float.MaxValue, maxY = float.MinValue;

        HashSet<DataPoint<DateTime, float>> set = new HashSet<DataPoint<DateTime, float>>();

        foreach (DataPoint<DateTime, float> point in data)
        {
            if (set.Contains(point)) continue;

            if (point.yValue < minY)
            {
                minY = point.yValue;
            }

            if (point.yValue > maxY)
            {
                maxY = point.yValue;
            }

            set.Add(point);
        }

        GameObject[] points = new GameObject[set.Count];

        data = set.ToList();

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = Instantiate(point, pointContainer);
            float x = (float)((data[i].xValue - minX).TotalSeconds / (maxX - minX).TotalSeconds) * scalingFactor * (1920f / 1080f) * canvas.transform.localScale.x,
                y = (1080f / 1920f) * canvas.transform.localScale.y * scalingFactor * (data[i].yValue - minY) / (maxY - minY);
            points[i].transform.localPosition = new Vector3(x, y);

            if (i > 0)
            {
                GameObject lineObj = Instantiate(line, pointContainer);
                lineObj.transform.localPosition = (points[i].transform.localPosition + points[i - 1].transform.localPosition) / 2;
                lineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(10, Vector3.Distance(points[i].transform.localPosition, points[i - 1].transform.localPosition));

                float xDiff = points[i].transform.localPosition.x - points[i - 1].transform.localPosition.x,
                    yDiff = points[i].transform.localPosition.y - points[i - 1].transform.localPosition.y;

                lineObj.transform.rotation = Quaternion.Euler(0, 0, yDiff != 0 ? Mathf.Rad2Deg * -Mathf.Atan(xDiff / yDiff) : 0);
            }
        }
    }

    public void CreateLabels(List<DataPoint<DateTime, float>> data)
    {
        DateTime minX = data[0].xValue,
            maxX = data[data.Count - 1].xValue;

        float minY = float.MaxValue, maxY = float.MinValue;

        foreach (DataPoint<DateTime, float> point in data)
        {
            if (point.yValue < minY)
            {
                minY = point.yValue;
            }

            if (point.yValue > maxY)
            {
                maxY = point.yValue;
            }
        }

        //X labels
        for (int i = 0; i < 4; i++)
        {
            TextMeshProUGUI labelObj = Instantiate(label, labelContainer).GetComponent<TextMeshProUGUI>();
            labelObj.transform.localPosition = new Vector2(i * (scalingFactor / 3f) * 1920 / 1080 * canvas.transform.localScale.x, -50);
            labelObj.text = (minX.Add(TimeSpan.FromSeconds((maxX - minX).TotalSeconds * (i / (float)3)))).ToString();
        }

        //Y labels
        for (int i = 0; i < 4; i++)
        {
            TextMeshProUGUI labelObj = Instantiate(label, labelContainer).GetComponent<TextMeshProUGUI>();
            labelObj.transform.localPosition = new Vector2(-25, i * (scalingFactor / 3f) * 1080 / 1920 * canvas.transform.localScale.y);
            labelObj.text = Functions.RoundToDecimalPlaces(((maxY - minY) * (i / (float)3)) + minY, 2).ToString();
        }
    }
}
