//Imports
using System.Collections.Generic;
using UnityEngine;

//Class used to generate the lines
public class Generator : MonoBehaviour
{
    //Private instance variables which are visible in the inspector
    [SerializeField] private GameObject cube;
    [SerializeField] private Ball ball;
    [SerializeField] private Transform endPosition;

    //Private instance variable which are not visible in the inspector
    private List<Vector2> points;
    private List<GameObject> pointsInstantiated;

    //Method called when the scene loads
    private void Start()
    {
        //Creates an instance for the unassigned instance variables
        //Adds the first base point to the points list
        points = new List<Vector2>() { ball.transform.position - new Vector3(0, 1) };
        pointsInstantiated = new List<GameObject>();
    }

    //Method called to start the simulation
    public void StartSim()
    {
        //Generates the points from a list, and flags that the sim has started
        GenerateFromList(points);
        ball.hasStarted = true;
    }

    //Method called to end the simulation
    public void EndSim()
    {
        //Removes each point from the list
        while(points.Count > 0)
        {
            points.RemoveAt(0);
        }

        //Destroys and removes each instantiated point
        while(pointsInstantiated.Count > 0)
        {
            Destroy(pointsInstantiated[0]);
            pointsInstantiated.RemoveAt(0);
        }

        //Resets the two lists
        pointsInstantiated = new List<GameObject>();
        points = new List<Vector2>() { ball.transform.position - new Vector3(0, 1) };
    }

    //Method called to add the point to the generator
    public void AddPoint(Vector2 point)
    {
        //Adds the point to the list
        points.Add(point);
    }
    
    //Overloaded method for the one above, does the same thing but takes in two floats instead
    public void AddPoint(float x, float y)
    {
        points.Add(new Vector2(x, y));
    }

    //Method called to generate the lines corresponding to a list of points
    private void GenerateFromList(List<Vector2> points)
    {
        //Loops through every pair of points
        for(int i = 0; i < points.Count - 1; i++)
        {
            //Generates a line between the pair of points
            GenerateFrom2Points(points[i], points[i + 1]);
        }
    }

    //Method called to generate a line which connects two points
    private void GenerateFrom2Points(Vector2 point1, Vector2 point2)
    {
        //Poistion of the line is the average position of the two points
        Vector2 position = (point1 + point2) / 2;

        //Gets the differences between the x and y values
        float x = point1.x - point2.x,
            y = point1.y - point2.y;

        //If the points are not equal
        if (y != 0 || x != 0)
        {
            //Set a base value for the angle of the line
            float angle = 0;

            //Checks if the line has a slope (if y = 0, slope = 0, and angle = 0)
            if (y != 0)
            {
                //Calculates the angle using sohcahtoa
                angle = Mathf.Rad2Deg * Mathf.Atan(x / y);
            }

            //Instantiates the object into the scene, at the position, and with the angle on the z axis
            GameObject next = Instantiate(cube, position, Quaternion.Euler(new Vector3(0, 0, -angle)));

            //Stretches the line to connect the points
            next.transform.localScale = new Vector3(1, Vector3.Distance(point1, point2) * 1.25f);

            //Adds the line to the list
            pointsInstantiated.Add(next);
        }
    }
    
    //Method to get the points
    public List<Vector2> GetPoints() => points;

    //Method to get the last point
    public Vector2 LastPoint() => points[0];
}
