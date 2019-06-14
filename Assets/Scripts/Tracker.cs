using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Tracker : MonoBehaviour
{
    private int paramFOV = 300;
    private int camWidth;
    private int camHeight;

    void Start()
    {
        camWidth = Camera.main.pixelWidth;
        camHeight = Camera.main.pixelHeight;
    }

    void Update()
    {
        if (PereferalTest.targetIsFront)
        {
            GameObject test = UnityTargetRotation.GetTempTargetArray();

            //探索対象のオブジェクトの２次元位置を求める
            var target2dpos = Camera.main.WorldToScreenPoint(test.transform.position);

            //カメラの視野をpixelで求める
            //Debug.Log("target2dpos: " + target2dpos + "    camWidth: " + camWidth + "     camHeight: " + camHeight);

            //視野の-paramFOVpxから，+paramFOVpxまでの範囲にGameObjectが入っているかどうかを確認
            if (-paramFOV <= target2dpos.x && target2dpos.x <= camWidth + paramFOV && -paramFOV <= target2dpos.y && target2dpos.y <= camHeight + paramFOV)
            {
                //Debug.Log("target is in the FOV");
                //この場合、中心視を使う
                if (UnityTargetRotation.phase != 2 && (PlayModeSelecter.GetMode() == 4 || PlayModeSelecter.GetMode() == 0))
                {
                    //if (PereferalTest.targetIsFront)
                    //PereferalTest.arrow.SetActive(true);

                }
                //周辺視をOFF
                //SockertSend.SetDirectionFlag(false);
                UnityTargetRotation.viewPortFlag = true;
            }
            else
            {
                //Debug.Log("target is not in the FOV");
                //この場合、周辺視を使う
                UnityTargetRotation.viewPortFlag = false;
                //PereferalTest.arrow.SetActive(false);

                //周辺視をON
                if (PlayModeSelecter.GetMode()==2||PlayModeSelecter.GetMode()==4||PlayModeSelecter.GetMode()==0)
                    //SockertSend.SetDirectionFlag(true);
                    ;
                else
                    ;
                    //SockertSend.SetDirectionFlag(false);
            }
        }
    }
}
