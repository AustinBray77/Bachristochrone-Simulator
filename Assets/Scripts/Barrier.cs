using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for the barrier objects
public class Barrier : MonoBehaviour
{
    //Refrence to the ball in the local environment
    [SerializeField] private Ball ball;

    //Called when something interacts with the trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Restarts the environment if the colliding object is the ball
        if (collision.gameObject == ball.gameObject)
        {
            ball.Restart();
        }
    }
}
