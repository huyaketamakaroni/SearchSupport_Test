using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjManager : MonoBehaviour {

    void Awake()
    {
        if (Resident.sceneFlag > 2)
        {
            //自分を消す
            Destroy(this.gameObject);            
        }
        else
        {
            // 自分を消さない
            DontDestroyOnLoad(this);
            Resident.sceneFlag++;
        }
         
    }

}
