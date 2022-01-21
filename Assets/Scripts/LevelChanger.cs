using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevelChanger : MonoBehaviour
{
    public Animator animator;
    private string s;

    [SerializeField] private StartMenu menu;

    public void FadeToLevel(string scene) {
        s = scene;
        //animator.SetTrigger("FadeOut");
        SceneManager.LoadScene(s);
    }

    public void OnFadeComplete() {
        SceneManager.LoadScene(s);
    }

}
