using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private LevelChanger fade;
    [SerializeField] private Button graphButton, bestButton;

    private void Awake()
    {
        graphButton.interactable = File.Exists("output.txt");
        bestButton.interactable = File.Exists("best.txt");
    }

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
