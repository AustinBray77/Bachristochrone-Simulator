using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class End : MonoBehaviour
{
    [SerializeField] private Ball ball;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == ball.gameObject)
        {
            ball.Restart();
        }
    }
}
