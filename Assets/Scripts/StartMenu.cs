using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    //loads each scene when the method is run 
    public void StartML() {
        SceneManager.LoadScene("SampleScene");
    }

    public void BestResults() {
        SceneManager.LoadScene("ExampleScene");
    }

    public void LoadGraph() {
        SceneManager.LoadScene("Graphs");
    }

}
