using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
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
