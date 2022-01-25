using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using UnityEngine.SceneManagement;

//Class to control the ball object
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Agent
{
    //Public visible instance variables
    public bool useAI;

    //Public non visible instance variables
    [HideInInspector] public bool hasStarted;
    [HideInInspector] public float time;
    [HideInInspector] public List<float> l10Times;
    [HideInInspector] public bool offLeftOrBottom;

    //Private visible instance variables
    [SerializeField] private float maxPoints;
    [SerializeField] private Generator gen;
    [SerializeField] private Transform endPosition;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private List<Vector2> bestPoints;

    //Private non visible instance variables
    private Vector2 placementBounds;
    private BehaviorParameters behaviorParameters;
    private Rigidbody2D rb;
    private float pointCount;
    private Vector3 originalPosition;
    private float bestTime;

    //Static list to store the times the AI achieves.
    private static List<DataPoint<System.DateTime, float>> bestTimes;

    //Method called on scene instantiation
    private void Start()
    {
        //Sets all variables to default values
        l10Times = new List<float>();
        bestTimes = new List<DataPoint<System.DateTime, float>>();
        time = 0;
        pointCount = 0;
        bestTime = 10;
        hasStarted = false;
        offLeftOrBottom = false;
        rb = GetComponent<Rigidbody2D>();
        behaviorParameters = GetComponent<BehaviorParameters>();
        rb.gravityScale = 0;
        originalPosition = transform.position;
        placementBounds = new Vector2(
            (transform.localPosition.x - endPosition.localPosition.x) / 2,
            (transform.localPosition.y - endPosition.localPosition.y) / 2);
    }

    //Method called each frame
    private void Update()
    {
        //If the user hit space, start the simulation
        if ((Input.GetKey(KeyCode.Space) || (SceneManager.GetActiveScene().name == "BestPath" && !useAI)) && !hasStarted)
        {
            //Starts the simulation
            gen.StartSim();
            hasStarted = true;
            rb.gravityScale = 1;
        }
        //If the user hit the left mouse button place a point
        else if (Input.GetMouseButton(0) && SceneManager.GetActiveScene().name != "BestPath" && !useAI)
        {
            //Converts mouse pos to world pos
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Places a point at that position
            gen.AddPoint(point);
        }

        //If the episode has started...
        if (hasStarted)
        {
            //Increment time by the amount of time it took to render the last frame
            time += Time.deltaTime;

            //Restart if the episode has been running for longer than 10 seconds (assume the ball cannot reach the end)
            if (time > 10)
            {
                Restart();
            }
        }

        //Trigger if the ball is out of bounds
        if (transform.position.x > gen.transform.position.x + bounds.x || transform.position.x < gen.transform.position.x - bounds.x ||
            transform.position.y < gen.transform.position.y - bounds.y || transform.position.y > gen.transform.position.y + bounds.y)
        {
            //Restart the episode
            offLeftOrBottom = true;
            Restart();
        }
    }

    //Method called when the AI requests an action
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        //If not using AI, return
        if (!useAI)
        {
            //Debug.LogWarning("The AI is not being used");
            return;
        }

        //Decide how complex the NN is
        bool inp3Condition = vectorAction.ContinuousActions[0] >= 0.5 && behaviorParameters.BrainParameters.NumActions == 3;
        bool inp4Condition = vectorAction.ContinuousActions[0] != 0 && behaviorParameters.BrainParameters.NumActions == 4;

        //Check if the AI wants to place a point
        if ((inp3Condition || inp4Condition) && !hasStarted && maxPoints > pointCount)
        {
            //Get the position the AI wants to place at
            float x = vectorAction.ContinuousActions[1];
            float y = vectorAction.ContinuousActions[2];

            //Sets them to 0 if they are invalid numbers
            x = (float.IsNaN(x) || float.IsInfinity(x) || float.IsNegativeInfinity(x)) ? 0 : x;
            y = (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) ? 0 : y;

            //Adds the point and increments point counter
            gen.AddPoint(new Vector2(
                gen.transform.position.x + x * placementBounds.x + (transform.localPosition.x + endPosition.localPosition.x),
                gen.transform.position.y + y * placementBounds.y + (transform.localPosition.y + endPosition.localPosition.y)
                ));
            pointCount++;
        }
        //Else start the simulation if it hasn't already started
        else if (!hasStarted)
        {
            gen.StartSim();
            rb.gravityScale = 1;
        }
    }

    //Called when the AI requests observations
    public override void CollectObservations(VectorSensor sensor)
    {
        //Return if AI is not being used
        if (!useAI) return;

        //Add the ball positon and end position to the observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(endPosition.position);

        //Adds last point postion
        sensor.AddObservation(gen.LastPoint());
    }

    //Method to restart the training environment
    public void Restart()
    {
        //Sets the environment to have not started
        hasStarted = false;

        //If AI is being used, calculated reward and write results
        if (useAI)
        {
            //Gets the distance from the current position to the end position
            float distance = Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position));

            //If the ball is out of bounds set the reward to the negative distance minus 5
            if (offLeftOrBottom)
            {
                SetReward(-distance - 5);
            }
            //Else if time is greater than or equal to 10, set the reward to the negative distance
            else if (time >= 10f)
            {
                SetReward(-distance);
            }
            //Else if there are previous times to rank based off of
            else if (l10Times.Count > 0)
            {
                //Calculates the mean time of the last ten times
                float averageTime = 0;
                for (int i = 0; i < l10Times.Count; i++)
                {
                    averageTime += l10Times[i];
                }

                //Set the reward to 7 plus the difference between the current time and average time (lower time is better)
                SetReward(7 + averageTime - time);

                //If l10 times is equal to ten items remove the least recent item
                if (l10Times.Count == 10)
                {
                    l10Times.RemoveAt(0);
                }
            }
            //Else set the reward to the reciprocal of the time
            else
            {
                SetReward(1 / time);
            }

            //End the AI episode
            EndEpisode();

            //If the ball does not travel out of bounds and completed the course in under ten seconds
            if (!offLeftOrBottom && time < 10)
            {
                //If the time is new best time
                if (time < bestTime)
                {
                    //Set the best time and best points
                    bestTime = time;
                    bestPoints = gen.GetPoints();
                    Debug.Log("New best time! Time:" + time);

                    //Opens the best.txt to write to
                    StreamWriter file = new StreamWriter("best.txt", append: true);

                    //Adds the best time to the data to write
                    string line = bestTime.ToString() + "\n";

                    //Adds each points relative position to the generator to the data to write
                    foreach (Vector2 point in bestPoints)
                    {
                        line += Functions.PointToString((Vector3)point - gen.transform.position) + " ";
                    }
                    //Replaces the last character with a end line
                    line = line.Substring(0, line.Length - 1) + '\n';

                    //Writes the data to the file and closes
                    file.WriteLine(line);
                    file.Close();
                }

                //Adds the time to l10 times and best times
                l10Times.Add(time);
                bestTimes.Add(new DataPoint<System.DateTime, float>(System.DateTime.Now, time));

                //If best times has 100000 items, write the results and clear the cache by resetting the list
                if (bestTimes.Count > 100000)
                {
                    WriteResults();
                    bestTimes = new List<DataPoint<System.DateTime, float>>();
                }
            }
        }

        //Sets variables to their default values
        time = 0;
        pointCount = 0;
        offLeftOrBottom = false;
        transform.position = originalPosition;

        //Tells the generator that the sim has ended
        gen.EndSim();

        //Resets the rigidbody to its original state
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
    }

    //Method called when the application is to be quit
    public void OnApplicationQuit()
    {
        //If the user is running the training scene, write the results
        if (SceneManager.GetActiveScene().name != "SampleScene")
        {
            WriteResults();
        }
    }

    //Method called to write the results to output.txt
    private void WriteResults()
    {
        Debug.Log("Writing results to output.txt");
        //Opens the file, set to append data to the file
        StreamWriter file = new StreamWriter("output.txt", append: true);

        //Variable for the data to write
        string line = "";

        //Adds each data point to the data to write
        foreach (var n in bestTimes)
        {
            line += n + "\n";
        }

        //Writes the data and closes the stream writer
        file.WriteLine(line);
        file.Close();
    }
}
