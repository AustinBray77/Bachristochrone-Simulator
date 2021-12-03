using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : Agent
{
    [HideInInspector] public bool hasStarted;
    [HideInInspector] public float time;
    [HideInInspector] public List<float> l10Times;
    [HideInInspector] public bool offLeftOrBottom;

    private Rigidbody2D rb;
    private float pointCount;

    [SerializeField] private Vector2 bounds;
    [SerializeField] private float maxPoints;
    [SerializeField] private Generator gen;
    [SerializeField] private Transform endPosition;

    private void Start()
    {
        l10Times = new List<float>();
        time = 0;
        pointCount = 0;
        hasStarted = false;
        offLeftOrBottom = false;

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void Update()
    {
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

        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    hasStarted = true;
        //    rb.gravityScale = 1;
        //}
    }

    public override void OnActionReceived(ActionBuffers vectorAction)
    {
        if (vectorAction.ContinuousActions[0] != 0 && !hasStarted && maxPoints > pointCount)
        {
            float x = vectorAction.ContinuousActions[1];

            /* if(vectorAction.ContinuousActions[1] > 7)
            {
                x = 7;
            } else if (vectorAction.ContinuousActions[1] < -7)
            {
                x = -7;
            } */

            float y = vectorAction.ContinuousActions[2];

            /* if (vectorAction.ContinuousActions[2] > 7)
            {
                y = 7;
            }
            else if (vectorAction.ContinuousActions[2] < -7)
            {
                y = -7;
            } */

            gen.AddPoint(new Vector3(x * bounds.x, y * bounds.y) + gen.transform.position);
            pointCount++;
        }
        else if(!hasStarted)
        {
            gen.StartSim();
            rb.gravityScale = 1;
        }
    }
     /*
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxis("Fire1");
        continuousActions[1] = Input.GetAxis("Horizontal") * 7;
        continuousActions[2] = Input.GetAxis("Vertical") * 7;
        continuousActions[3] = Input.GetAxis("Fire2");
        
        if(continuousActions[0] > 7)
        {
            continuousActions[0] = 7;
        } else if(continuousActions[0] < -7)
        {
            continuousActions[0] = -7;
        }

        if (continuousActions[1] > 7)
        {
            continuousActions[1] = 7;
        }
        else if (continuousActions[1] < -7)
        {
            continuousActions[1] = -7;
        }

        //Debug.Log("Calculating output");
        //Debug.Log("Actions Decided: " + actionsOut);
    }*/

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(endPosition.position);

        

        /*
        Debug.Log("Setting observations");
        Debug.Log(transform.position);
        Debug.Log(endPosition.position);*/

        /*List<Vector2> points = gen.GetPoints();

        foreach(Vector2 point in points)
        {
            sensor.AddObservation(point);
        }*/
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "End")
    //    {
    //        Debug.Log("Here");
    //        Restart();
    //    }
    //}

    public void Restart()
    {
        hasStarted = false;

        if(offLeftOrBottom)
        {
            SetReward(-Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position)) - 5);
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
                SetReward(-Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position)));
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
                SetReward(-Mathf.Abs(Vector3.Distance(transform.position, endPosition.transform.position)));
            }
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
        transform.position = gen.transform.position + new Vector3(-5, 5);

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.velocity = new Vector2(0, 0);
    }
}
