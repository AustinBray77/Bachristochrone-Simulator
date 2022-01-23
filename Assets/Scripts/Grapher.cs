using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Grapher : MonoBehaviour
{
    [SerializeField] private float scalingFactor;
    [SerializeField] private Transform canvas, pointContainer, labelContainer;
    [SerializeField] private GameObject point, line, label;

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
            points[i].transform.localPosition = new Vector3((float)((data[i].xValue - minX).TotalSeconds / (maxX - minX).TotalSeconds) * scalingFactor * (1920f / 1080f) * canvas.transform.localScale.x, (1080f / 1920f) * canvas.transform.localScale.y * scalingFactor * (data[i].yValue - minY) / (maxY - minY));

            if (i > 0)
            {
                GameObject lineObj = Instantiate(line, pointContainer);
                lineObj.transform.localPosition = (points[i].transform.localPosition + points[i - 1].transform.localPosition) / 2;
                lineObj.GetComponent<RectTransform>().sizeDelta = new Vector2(10, Vector3.Distance(points[i].transform.localPosition, points[i - 1].transform.localPosition));

                float x = points[i].transform.localPosition.x - points[i - 1].transform.localPosition.x,
                    y = points[i].transform.localPosition.y - points[i - 1].transform.localPosition.y;

                lineObj.transform.rotation = Quaternion.Euler(0, 0, y != 0 ? Mathf.Rad2Deg * -Mathf.Atan(x / y) : 0);
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
            labelObj.transform.localPosition = new Vector2(i * (scalingFactor / 3f) * 1920 / 1080, -35);
            labelObj.text = (minX.Add(TimeSpan.FromSeconds((maxX - minX).TotalSeconds / (4 - i)))).ToString();
        }

        //Y labels
        for (int i = 0; i < 4; i++)
        {
            TextMeshProUGUI labelObj = Instantiate(label, labelContainer).GetComponent<TextMeshProUGUI>();
            labelObj.transform.localPosition = new Vector2(-25, i * (scalingFactor / 3f) * 1080 / 1920);
            labelObj.text = Functions.RoundToDecimalPlaces((((maxY - minY) / (4 - i)) + minY), 2).ToString();
        }
    }
}
