using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapher : MonoBehaviour
{
    [SerializeField] private float scalingFactor;
    [SerializeField] private Transform canvas;
    [SerializeField] private Sprite point, line;

    public void DrawTimeLineGraph(List<DataPoint<DateTime, float>> data)
    {
        if(data == null) return;
        if(data.Count < 1) return;

        GameObject pointContainer = new GameObject("Point container");
        pointContainer.transform.parent = canvas;
        GameObject[] points = new GameObject[data.Count];

        DateTime minX = data[0].xValue,
            maxX = data[data.Count - 1].xValue;
        
        float minY = data[0].yValue,
            maxY = data[data.Count - 1].yValue;

        for(int i = 0; i < points.Length; i++) 
        {
            points[i] = new GameObject("Line Graph Point: " + i, typeof(Image));
            points[i].transform.parent = pointContainer.transform;
            points[i].GetComponent<Image>().sprite = point;
            points[i].transform.position = new Vector3((float)((data[i].xValue - minX).TotalSeconds /(maxX - minX).TotalSeconds) * scalingFactor, scalingFactor * (data[i].yValue - minY) /(maxY - minY));

            GameObject lineObj = new GameObject("Line " + i, typeof(Image));
            lineObj.transform.parent = pointContainer.transform;
            lineObj.GetComponent<Image>().sprite = line;
        }
    }
}
