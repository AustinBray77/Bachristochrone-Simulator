using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartMenu : UI
{
    [SerializeField] private Button graphButton, bestButton;
    public Animator animator;


    private void Awake()
    {
        graphButton.interactable = File.Exists("output.txt");
        bestButton.interactable = File.Exists("best.txt");
    }

    //loads each scene when the method is run 
    public void StartML()
    {
        StartCoroutine(LoadLevel(1));
    }

    public void BestResults()
    {
        StartCoroutine(LoadLevel(0));
    }

    public void CustomPath()
    {
        StartCoroutine(LoadLevel(4));
    }

    public void LoadGraph()
    {
        StartCoroutine(LoadLevel(3));
    }

    IEnumerator LoadLevel(int i)
    {
        animator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        SceneManager.LoadScene(i);
    }

    public override void Back()
    {
        StartCoroutine(LoadLevel(2));
    }
}
