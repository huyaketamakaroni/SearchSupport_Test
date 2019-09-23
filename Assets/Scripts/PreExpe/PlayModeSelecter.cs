using UnityEngine;
using System.Collections;

public class PlayModeSelecter : MonoBehaviour {


    /*
    //裸眼探索モード
    public static bool bareEyeFlag;
    //周辺視ガイド付き探索モード
    public static bool ledGuideFlag;
    //中心視ガイド付き探索モード
    public static bool cgGuideFlag;
    //周辺視＋中心視ガイド付き探索モード
    public static bool bothGuideFlag;
    */

    /* 1 裸眼探索モードをONに
     * 2 周辺視ガイド付き探索モードをONに 
     * 3 中心視ガイド付き探索モード
     * 4 中心視ガイド付き探索モード
     */
    private static int mode;

    // Use this for initialization
    void Start () {
        //最初は裸眼からスタート
        mode = 0;
    }
	
	// Update is called once per frame 
	void Update () {
        //数字1 裸眼探索モードをONに
        //このときCGをOFFに
        /*
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            mode = 1;
            UnityTargetRotation.ResetTempTargetArray(mode);
        }
        //数字2 周辺視ガイド付き探索モードをONに 
        //このときCGをOFFに
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            mode = 2;
            UnityTargetRotation.ResetTempTargetArray(mode);
        }

        //数字3 中心視ガイド付き探索モード
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            mode = 3;
            UnityTargetRotation.ResetTempTargetArray(mode);
          }
        //数字4 中心視ガイド付き探索モード
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            mode = 4;
            UnityTargetRotation.ResetTempTargetArray(mode);
        }
        //数字0 練習モードをONに
        //このときCGをOFFに
        else if (Input.GetKeyUp(KeyCode.Alpha0))
        {
            mode = 0;
            UnityTargetRotation.ResetTempTargetArray(mode);
        }
        */
    }

    public static int GetMode()
    {
        return mode;
    }
}
