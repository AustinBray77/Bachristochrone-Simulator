using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private List<Vector2> pointsToSetTo;
    private List<Vector2> points;
    private List<GameObject> pointsInstantiated;

    [SerializeField] private GameObject cube;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform endPosition;

    private void Start()
    {
        points = new List<Vector2>(pointsToSetTo);
        pointsInstantiated = new List<GameObject>();
    }

    public void StartSim()
    {
        GenerateFromList(points);
        ball.hasStarted = true;
    }

    public void EndSim()
    {
        while(points.Count > 0)
        {
            points.RemoveAt(0);
        }

        while(pointsInstantiated.Count > 0)
        {
            Destroy(pointsInstantiated[0]);
            pointsInstantiated.RemoveAt(0);
        }

        pointsInstantiated = new List<GameObject>();
        points = new List<Vector2>(pointsToSetTo);
    }

    public void AddPoint(Vector2 point)
    {
        points.Add(point);
    }
    
    public void AddPoint(float x, float y)
    {
        points.Add(new Vector2(x, y));
    }

    private void GenerateFromList(List<Vector2> points)
    {
        for(int i = 0; i < points.Count - 1; i++)
        {
            GenerateFrom2Points(points[i], points[i + 1]);
        }
    }

    private void GenerateFrom2Points(Vector2 point1, Vector2 point2)
    {
        Vector2 position = (point1 + point2) / 2;

        float x = point1.x - point2.x,
            y = point1.y - point2.y;

        if (y != 0 || x != 0)
        {
            float angle = 0;

            if (y != 0)
            {
                angle = Mathf.Rad2Deg * Mathf.Atan(x / y);
            }

            GameObject next = Instantiate(cube, position, Quaternion.Euler(new Vector3(0, 0, -angle)));

            next.transform.localScale = new Vector3(1, Vector3.Distance(point1, point2) * 1.25f);

            pointsInstantiated.Add(next);
        }
    }
    
    public List<Vector2> GetPoints()
    {
        return points;
    }
}
