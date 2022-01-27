using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

//Class to control the start menu elements
public class StartMenu : UI
{
    //Instance refrences to UI elements
    [SerializeField] private Button graphButton, bestButton;

    //Refrence to the animator
    public Animator animator;

    //Method called on scene start
    private void Awake()
    {
        //Sets if the buttons are interactable if the files required for those scenes exist
        graphButton.interactable = File.Exists("output.txt");
        bestButton.interactable = File.Exists("best.txt");
    }

    //Method to load the machine learning scene
    public void StartML()
    {
        StartCoroutine(LoadLevel(1));
    }

    //Method to load the best results scene
    public void BestResults()
    {
        StartCoroutine(LoadLevel(2));
    }

    //Method to load the custom path scene
    public void CustomPath()
    {
        StartCoroutine(LoadLevel(4));
    }

    //Method to laod the graph scene
    public void LoadGraph()
    {
        StartCoroutine(LoadLevel(3));
    }

    //Method to load a level from its index
    private IEnumerator LoadLevel(int i)
    {
        //Sets the trigger in the animator to start the animation
        animator.SetTrigger("Start");

        //Waits a second then loads the scene
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(i);
    }

    //Overriden method to go back to the start scene
    public override void Back()
    {
        StartCoroutine(LoadLevel(0));
    }
}
