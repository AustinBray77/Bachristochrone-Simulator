using System.IO;
using System.Collections;
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
    [HideInInspector] public bool hasStarted;
    [HideInInspector] public float time;
    [HideInInspector] public List<float> l10Times;
    [HideInInspector] public bool offLeftOrBottom;

    private Rigidbody2D rb;
    private float pointCount;
    private Vector3 originalPosition;

    [SerializeField] private float maxPoints;
    [SerializeField] private Generator gen;
    [SerializeField] private Transform endPosition;
    [SerializeField] private bool useAI;
    [SerializeField] private Vector2 bounds;
    private Vector2 placementBounds;
    private BehaviorParameters behaviorParameters;

    //private static string line = "";
    private static List<float> bestTimes;
    private float bestTime;
    [SerializeField] private List<Vector2> bestPoints;

    //Method called on scene instantiation
    private void Start()
    {
        //Sets all variables to default values
        l10Times = new List<float>();
        time = 0;
        pointCount = 0;
        bestTime = 10;
        hasStarted = false;
        offLeftOrBottom = false;
        rb = GetComponent<Rigidbody2D>();
        behaviorParameters = GetComponent<BehaviorParameters>();
        rb.gravityScale = 0;
        originalPosition = transform.position;
        placementBounds = new Vector2((transform.position.x - endPosition.position.x) / 2,
        (transform.position.y - endPosition.position.y) / 2);
        Debug.Log(SceneManager.GetActiveScene().name);
    }

    //Method called each frame
    private void Update()
    {
        //If the user hit space, start the simulation
        if((Input.GetKey(KeyCode.Space) || (SceneManager.GetActiveScene().name == "ExampleScene" && !useAI)) && !hasStarted)
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
            
            //Adds the point and increment point counter
            gen.AddPoint(new Vector3(x * placementBounds.x  + (transform.position.x + endPosition.position.x), y * placementBounds.y  + (transform.position.y + endPosition.position.y)) + gen.transform.position);
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
        sensor.AddObservation(gen.LastPoint());
    }

    public void Restart()
    {
        if (!offLeftOrBottom && time < 10)
        {
            if(time < bestTime)
            {
                bestTime = time;
                bestPoints = gen.GetPoints();
                Debug.Log("New best time! Time:" + time);

                foreach (Vector2 point in bestPoints)
                {
                    Debug.Log(point.x + ", " + point.y);
                }
            }
        }

        hasStarted = false;
        float distance = Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position));

        if (offLeftOrBottom)
        {
            SetReward(-distance - 5);
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
            } else
            {
                SetReward(-distance);
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
            } else
            {
                SetReward(-distance);
            }
        }

        EndEpisode();

        if (time < 10)
        {
            l10Times.Add(time);
            bestTimes.Add(time);
            SortBestTimes();
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
        if (SceneManager.GetActiveScene().name != "ExampleScene")
        {
            WriteResults();
        }
    }

    private static void SortBestTimes() 
    {
        for(int i = 1; i < bestTimes.Count; i++) 
        {
            float n = bestTimes[i];
            int index = i-1;

            while(index >= 0 && bestTimes[index] > n) 
            {
                bestTimes[index+1] = bestTimes[index];
                index--;
            }

            bestTimes[index] = n;
        }
    }

    private void WriteResults()
    {
        Debug.Log("Writing results to output.txt");
        using StreamWriter file = new StreamWriter("output.txt", append: true);
        
        string line = "";

        foreach(float n in bestTimes) {
            line += n + "\n";
        }

        file.WriteLine(line);
        file.Close();
    }
}
