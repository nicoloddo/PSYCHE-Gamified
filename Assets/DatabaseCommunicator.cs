using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseCommunicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendData(string function)
    {
        /*
            List of functions that are passed:
            AIterminated()
            AIcontinue()
            ProgressBarExpired()
        */
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalEval(function);
        #endif

        Debug.Log("Sending data from Unity: " + function);
    }
}
