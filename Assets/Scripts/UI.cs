using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Class to control all UI
public class UI : MonoBehaviour
{
    //Base method to load back to the start scene
    public virtual void Back()
    {
        //Loads the start scene
        SceneManager.LoadScene(2);
    }
}
