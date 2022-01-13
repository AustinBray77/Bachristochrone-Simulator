using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grapher : MonoBehaviour
{
    [SerializeField] private Transform canvas;
    [SerializeField] private Sprite point, line;

    public void DrawLineGraph<T, U>(List<DataPoint<T, U>> data)  where T : System.IComparable where U : System.IComparable
    {
        GameObject pointContainer = new GameObject("Point container");
        pointContainer.transform.parent = canvas;
        GameObject[] points = new GameObject[data.Count];

        for(int i = 0; i < points.Length; i++) {
            points[i] = new GameObject("Line Graph Point: " + i, typeof(Image));
            points[i].transform.parent = pointContainer.transform;
            points[i].GetComponent<Image>().sprite = point;
        }
    }
}
