using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMananager : MonoBehaviour {

    public void Load_A()
    {       
        SceneManager.LoadScene("Main");

    }

    public void Load_Start()
    {

        SceneManager.LoadScene("Start");
    }
}
