using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMode : MonoBehaviour {

    public GameObject StartObj_A; 

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RayCastTest.CheckRaycast();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (RayCastTest.GetSelectedGameObject() == StartObj_A)
            {
                Debug.Log("Start");
                UnityEngine.SceneManagement.SceneManager.LoadScene("Main");
            }

            Debug.Log("Key: Space");
        }

        
    }
}
