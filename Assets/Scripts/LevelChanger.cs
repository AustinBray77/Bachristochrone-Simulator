using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private string s;

    StartMenu menu = FindObjectOfType<StartMenu>();
    public void FadeToLevel(string scene) {
        s = scene;
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() {
        SceneManager.LoadScene(s);
    }

}
