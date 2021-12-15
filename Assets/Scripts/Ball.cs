using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

//Class to control the ball object
[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Agent
{
    //Instance variables
    [HideInInspector] public bool hasStarted; //if the episode has started
    [HideInInspector] public float time; //current time in the episode
    [HideInInspector] public List<float> l10Times; //Last 10 successful runs
    [HideInInspector] public bool offLeftOrBottom; //if the ball went out of bounds

    private Rigidbody2D rb; // Refrence to the rigidbody on the ball
    private float pointCount; //Number of points that have been placed
    private Vector3 originalPosition; //Original position of the ball

    [SerializeField] private Vector2 bounds; //Bounds for the environment
    [SerializeField] private float maxPoints; //Max number of points the can be placed
    [SerializeField] private Generator gen; //Refrence to the generator in the environment
    [SerializeField] private Transform endPosition; //End position the ball is trying to reach
    [SerializeField] private bool useAI; //If the environment is to use AI
    
    private BehaviorParameters behaviorParameters; //Refrence to the behaviour parameters on the ball
    
    //Static variables
    private static string line = ""; //Buffer for writing to the file

    //Method called on scene instantiation
    private void Start()
    {
        //Sets all variables to default values
        l10Times = new List<float>();
        time = 0;
        pointCount = 0;
        hasStarted = false;
        offLeftOrBottom = false;
        rb = GetComponent<Rigidbody2D>();
        behaviorParameters = GetComponent<BehaviorParameters>();
        rb.gravityScale = 0;
        originalPosition = transform.position;
    }

    //Method called each frame
    private void Update()
    {
        //If the user hit space, start the simulation
        if(Input.GetKey(KeyCode.Space) && !hasStarted)
        {
            //Starts the simulation
            gen.StartSim();
            hasStarted = true;
            rb.gravityScale = 1;
        } 
        //If the user hit the left mouse button place a point
        else if(Input.GetMouseButton(0))
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
        if(!useAI)
        {
            Debug.LogWarning("The AI is not being used");
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
            
            //Adds the point and increment point counter
            gen.AddPoint(new Vector3(x * bounds.x, y * bounds.y) + gen.transform.position);
            pointCount++;
        }
        //Else start the simulation if it hasn't already started
        else if(!hasStarted)
        {
            gen.StartSim();
            rb.gravityScale = 1;
        }
    }
    
    //Called when the AI requests observations
    public override void CollectObservations(VectorSensor sensor)
    {
        //Add the ball positon and end position to the observations
        sensor.AddObservation(transform.position);
        sensor.AddObservation(endPosition.position);
        
        /* Add last point postion */
    }

    public void Restart()
    {
        hasStarted = false;
        float distance = Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position));

        if (offLeftOrBottom)
        {
            SetReward(-distance - 5);
            line += "~";
        } else if (l10Times.Count > 0)
        {
            float averageTime = 0;

            for (int i = 0; i < l10Times.Count; i++)
            {
                averageTime += l10Times[i];
            }

            if (time < 10f)
            {
                SetReward(7 + averageTime - time);
                line += "*";
            } else
            {
                SetReward(-distance);
                line += "!";
            }

            if (l10Times.Count == 10)
            {
                l10Times.RemoveAt(0);
            }
        }
        else
        {
            if (time < 10)
            {
                SetReward(1 / time);
                line += "*";
            } else
            {
                SetReward(-distance);
                line += "!";
            }
        }

        line += "Reward:" + GetCumulativeReward().ToString()+" Time:"+time.ToString()+"\n";

        if(line.Length > 1000000)
        {
            WriteResults();
        }

        EndEpisode();

        if (time < 10)
        {
            l10Times.Add(time);
        }

        gen.EndSim();

        time = 0;
        pointCount = 0;

        offLeftOrBottom = false;
        transform.position = originalPosition;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
    }

    public void OnApplicationQuit()
    {
        WriteResults();
    }

    private void WriteResults()
    {
        Debug.Log("Writing results to output.txt");
        using StreamWriter file = new StreamWriter("output.txt", append: true);
        file.WriteLine(line);
        file.Close();
        line = "";
    }
}
