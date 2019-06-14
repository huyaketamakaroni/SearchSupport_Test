using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class FileOutput : MonoBehaviour {

    //経過時間
    private static float duration;
  
	// Use this for initialization
	void Start () {
        //時間の初期化
        duration = 0;
    }
	
	// Update is called once per frame
	void Update () {
        //Time.deltaTime(最後のフレームを完了するのに要した時間)を追加
        duration += Time.deltaTime;
    }

    public static float GetDuration()
    {
        return duration;
    }

    public static void ResetDuration()
    {
        duration = 0.0f;
    }

}
