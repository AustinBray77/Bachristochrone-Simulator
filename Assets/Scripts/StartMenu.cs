using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private LevelChanger fade;

    //loads each scene when the method is run 
    public void StartML()
    {
        fade.FadeToLevel("SampleScene");
    }

    public void BestResults()
    {
        fade.FadeToLevel("ExampleScene");
    }

    public void CustomPath()
    {
        fade.FadeToLevel("UserControlled");
    }

    public void LoadGraph()
    {
        fade.FadeToLevel("Graphs");
    }

}
