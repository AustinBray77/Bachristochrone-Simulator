using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class used for the end goal object
public class End : MonoBehaviour
{
    //Refrence to the ball in the environment
    [SerializeField] private Ball ball;

    //Method called when something touches the end goal
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Restart the episode if the object was the ball
        if(collision.gameObject == ball.gameObject)
        {
            ball.Restart();
        }
    }
}
