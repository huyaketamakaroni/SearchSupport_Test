using UnityEngine;
using System.Collections;

public class RayCastTest : MonoBehaviour {

    //RayCastによって計算されたオブジェクトを格納するための変数
    private static GameObject selectedGameObject = null;
 
	// Update is called once per frame
	public static void CheckRaycast () {
        //頭部の位置
        var headPosition = Camera.main.transform.position;
        //頭部の姿勢
        var gazePosition = Camera.main.transform.forward;

        //RayCastがヒットしたときの情報が格納される
        RaycastHit hit;

        //カーソルがオブジェクトにあたった場合
        //if (Physics.Raycast(headPosition, gazePosition, out hit))
        //Rayの太さを変える場合はこちら(単位はm)
        if (Physics.SphereCast(headPosition, 0.001f, gazePosition, out hit))
        {
            //Debug.Log(hit.collider.name);
            //変数selectedGameObjectにあたったgameObjectを格納
            selectedGameObject = hit.collider.gameObject;
        }
        else
        {
            selectedGameObject = GameObject.Find("dummy");
            //Debug.Log(selectedGameObject);
        }
    }

    public static GameObject GetSelectedGameObject()
    {
        return selectedGameObject;
    }
}
