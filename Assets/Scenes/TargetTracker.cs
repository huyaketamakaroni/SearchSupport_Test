using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetTracker : MonoBehaviour
{
    // -- 外部設定変数 ---------------------------- //
    [Header("[利用GameObject設定]")]
    public static GameObject targetObject;
    public GameObject anywayObject;

    [Header("[Arrow CG 前方描画距離(m)]")]
    [Range(0.1f, 1.0f)] public float forwardDistance = 0.5f;

    [Header("[Arrow CG 色変化距離 閾値(m)]")]
    [Range(0.1f, 10.0f)] public float thresholdNear = 1.0f;
    [Range(0.1f, 10.0f)] public float thresholdMiddle = 1.5f;

    [Header("[AirTap 検出距離 閾値(m)]")]
    [Range(0.1f, 3.0f)] public float thresholdAirTap = 1.0f;

    public static bool isLook = false;


    // -- 内部設定変数 ---------------------------- //
    private Rect screenArea = new Rect(0, 0, 1, 1);

    private void Awake()
    {
        targetObject = anywayObject;
    }


    // -- Unity Update関数 ---------------------------- //
    void Update()
    {
        if (Input.GetKeyDown("o")){
            forwardDistance = forwardDistance + 0.05f;
        }
        if (Input.GetKeyDown("i"))
        {
            forwardDistance = forwardDistance - 0.05f;
        }


        Vector3 targetPoint = Camera.main.WorldToViewportPoint(targetObject.transform.position);

        float distance = (Camera.main.transform.position - targetObject.transform.position).sqrMagnitude;  // HoloLensと対象コンテナ間の距離

        Vector3 forward = forwardDistance * Camera.main.transform.TransformDirection(Vector3.forward);
        transform.position = Camera.main.transform.position + forward;
        transform.LookAt(targetObject.transform);


        // -- 距離に応じたオブジェクト色の変更 ---------------------------- //
        Color matColor;
        matColor = Color.red;


        // -- 距離に応じたオブジェクト直視判定 ---------------------------- //
        if (distance <= thresholdAirTap * thresholdAirTap)
            isLook = true;
        else
            isLook = false;


        // -- 表示領域に応じたオブジェクト表示判定 ---------------------------- //

        bool renderFlag = !screenArea.Contains(targetPoint);

        foreach (Transform child in transform)
        {
            GameObject gameObject = child.gameObject;
            Renderer renderer = gameObject.GetComponent<Renderer>();

            renderer.enabled = renderFlag;
            //renderer.material.color = matColor;
        }
    }


    // -- オブジェクト直視判定結果 外部取得メソッド ---------------------------- //
    public static bool getLookingState()
    {
        return isLook;
    }
}
