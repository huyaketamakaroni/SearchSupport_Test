using UnityEngine;
using System.Collections;

public class Line3DecidePos : MonoBehaviour {

    private GameObject[] currentObj;
    private int MAX = 6;
    // Use this for initialization
    void Start()
    {
        currentObj = new GameObject[MAX];
        currentObj[0] = GameObject.Find("ledCube2");
        currentObj[1] = GameObject.Find("bareCube2");
        currentObj[2] = GameObject.Find("cgCube3");
        currentObj[3] = GameObject.Find("bothCube3");
        currentObj[4] = GameObject.Find("ledCube3");
        currentObj[5] = GameObject.Find("bothCube4");
    }

    void Update()
    {
        if (this.gameObject == RayCastTest.GetSelectedGameObject())
        {
            //Debug.Log("raycastによってオブジェクトが選択されました。移動する際はキーボード入力を用いてください。");
            /*Yの位置姿勢を変更する*/
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.y += 0.01f;
                    currentObj[i].transform.position = pos;
                }
                //Debug.Log("UpArrow");
            }
            //下矢印
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.y -= 0.01f;
                    currentObj[i].transform.position = pos;
                }

                //Debug.Log("DownArrow");
            }
            //C
            else if (Input.GetKeyDown(KeyCode.C))
            {
                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.x += 0.01f;
                    currentObj[i].transform.position = pos;
                }

                //Debug.Log("C");
            }
            //V
            else if (Input.GetKeyDown(KeyCode.V))
            {

                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.x -= 0.01f;
                    currentObj[i].transform.position = pos;
                }

                //Debug.Log("V");
            }
            //D
            else if (Input.GetKeyDown(KeyCode.D))
            {
                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.z += 0.01f;
                    currentObj[i].transform.position = pos;
                }

                //Debug.Log("D");
            }
            //F
            else if (Input.GetKeyDown(KeyCode.F))
            {

                for (int i = 0; i < MAX; i++)
                {
                    Vector3 pos = currentObj[i].transform.position;
                    pos.z -= 0.01f;
                    currentObj[i].transform.position = pos;
                }

                //Debug.Log("F");
            }



        }
        else
        {
            ;
        }
    }
}
