using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;

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

    [SerializeField] private Vector2 bounds;
    [SerializeField] private float maxPoints;
    [SerializeField] private Generator gen;
    [SerializeField] private Transform endPosition;
    [SerializeField] private bool useAI;
    private BehaviorParameters behaviorParameters;

    private static string line = "";

    private void Start()
    {
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

    private void Update()
    {
        if(Input.GetKey(KeyCode.Space) && !hasStarted)
        {
            gen.StartSim();
            hasStarted = true;
            rb.gravityScale = 1;
        } else if(Input.GetMouseButton(0))
        {
            Vector3 point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            gen.AddPoint(point);
        }

        if (hasStarted)
        {
            time += Time.deltaTime;

            if (time > 10)
            {
                Restart();
            }
        }

        if (transform.position.x > gen.transform.position.x + bounds.x || transform.position.x < gen.transform.position.x - bounds.x ||
            transform.position.y < gen.transform.position.y - bounds.y || transform.position.y > gen.transform.position.y + bounds.y)
        {
            offLeftOrBottom = true;
            Restart();
        }
    }

    
    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        if(!useAI)
        {
            Debug.LogWarning("The AI is not being used");
            return;
        }

        bool inp3Condition = vectorAction.ContinuousActions[0] >= 0.5 && behaviorParameters.BrainParameters.NumActions == 3;
        bool inp4Condition = vectorAction.ContinuousActions[0] != 0 && behaviorParameters.BrainParameters.NumActions == 4;

        if ((inp3Condition || inp4Condition) && !hasStarted && maxPoints > pointCount)
        {
            float x = vectorAction.ContinuousActions[1];
            float y = vectorAction.ContinuousActions[2];

            x = (float.IsNaN(x) || float.IsInfinity(x) || float.IsNegativeInfinity(x)) ? 0 : x;
            y = (float.IsNaN(y) || float.IsInfinity(y) || float.IsNegativeInfinity(y)) ? 0 : y;

            gen.AddPoint(new Vector3(x * bounds.x, y * bounds.y) + gen.transform.position);
            pointCount++;
        }
        else if(!hasStarted)
        {
            gen.StartSim();
            rb.gravityScale = 1;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(endPosition.position);
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
