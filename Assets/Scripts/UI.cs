using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{
    public virtual void Back()
    {
        SceneManager.LoadScene(2);
    }
}
