using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    //loads each scene when the method is run 
    public void StartML() {
        LevelChanger Fade = FindObjectOfType<LevelChanger>();
        Fade.FadeToLevel("SampleScene");
    }

    public void BestResults() {
        LevelChanger Fade = FindObjectOfType<LevelChanger>();
        Fade.FadeToLevel("ExampleScene");
    }

    public void LoadGraph() {
        LevelChanger Fade = FindObjectOfType<LevelChanger>();
        Fade.FadeToLevel("Graphs");
    }

}
