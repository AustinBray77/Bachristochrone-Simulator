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

    [SerializeField] private float maxPoints;
    [SerializeField] private Generator gen;
    [SerializeField] private Transform endPosition;
    [SerializeField] private bool useAI;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private List<Vector2> bestPoints;

    private Vector2 placementBounds;
    private BehaviorParameters behaviorParameters;
    private Rigidbody2D rb;
    private float pointCount;
    private Vector3 originalPosition;
    private float bestTime;

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
        placementBounds = new Vector2((transform.localPosition.x - endPosition.localPosition.x) / 2,
        (transform.localPosition.y - endPosition.localPosition.y) / 2);
        Debug.Log(placementBounds.x + " " + placementBounds.y);
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
            
            //Adds the point and increments point counter
            gen.AddPoint(new Vector2(
                gen.transform.position.x + x * placementBounds.x  + (transform.localPosition.x + endPosition.localPosition.x), 
                gen.transform.position.y + y * placementBounds.y  + (transform.localPosition.y + endPosition.localPosition.y)
                ));
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

        if (time < 10 && !offLeftOrBottom)
        {
            l10Times.Add(time);
            bestTimes.Add(new DataPoint<System.DateTime, float>(System.DateTime.Now, time));
        }

        time = 0;
        pointCount = 0;

        offLeftOrBottom = false;
        transform.position = originalPosition;

        gen.EndSim();

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

    private void QSortData<T, U>(List<DataPoint<T, U>> data, int min, int max) 
    where T : System.IComparable where U : System.IComparable
    {
        for(int i = min + 1, j = max; i < j;) {
                    
        }
    }

    private void Partition<T, U>(List<DataPoint<T, U>> data, int min, int max) 
    where T : System.IComparable where U : System.IComparable
    {
        int swap = min;
        DataPoint<T, U> holder;

        for(int i = min + 1; i < max; i++) {
            if(data[i] < data[min]) {
                swap++;
                
                holder = data[i];
                data[i] = data[swap];
                data[swap] = holder;
            }
        }

        holder = data[min];
        data[min] = data[swap];
        data[swap] = holder;
    }

    private void WriteResults()
    {
        Debug.Log("Writing results to output.txt");
        using StreamWriter file = new StreamWriter("output.txt");
        
        string line = "";

        foreach(var n in bestTimes) {
            line += n + "\n";
        }

        file.WriteLine(line);
        file.Close();
    }
}
