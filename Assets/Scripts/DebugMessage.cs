using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMessage : MonoBehaviour {

    TextMesh textMesh;

    public bool ignoreNormalLog;

    // Use this for initialization
    void Start () {

        textMesh = gameObject.GetComponentInChildren<TextMesh>();
	}

    void OnEnable()
    {
        Application.logMessageReceived += LogMessageCallback;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessageCallback;
    }

    public void LogMessageCallback(string message, string stackTrace, LogType type)
    {
        string trace = null;

        switch (type)
        {
            case LogType.Log:
                // Ignore normal log
                if (ignoreNormalLog)
                {
                    return;
                }
                break;
            case LogType.Warning:
                trace = stackTrace.Remove(0, (stackTrace.IndexOf("\n") + 1));
                break;
            case LogType.Error:
            case LogType.Assert:
                trace = stackTrace.Remove(0, (stackTrace.IndexOf("\n") + 1));
                break;
            case LogType.Exception:
                trace = stackTrace;
                break;

        }
        if (trace != null)
        {
            message = message + trace;
        }
        if (textMesh.text.Length > 300)
        {
            textMesh.text = message + "\n";
        }
        else
        {
            textMesh.text += message + "\n";
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
