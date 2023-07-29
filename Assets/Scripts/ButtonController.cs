using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private UIController uiController;
    public bool continue_click = false;
    public bool restart_click = false;
    public bool change_level_click = false;
    public bool reset_click = false;
    public bool terminate_click = false;
    public bool continueAI_click = false;

    // Start is called before the first frame update
    void Start()
    {
        uiController = FindObjectOfType<UIController>();
        continue_click = false;
        restart_click = false;
        change_level_click = false;
        reset_click = false;
        terminate_click = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnContinueClick()
    {
        continue_click = true;
    }

    public void OnTerminateAIClick()
    {
        terminate_click = true;
    }

    public void OnContinueAIClick()
    {
        continueAI_click = true;
    }

    public void OnRestartClick()
    {
        restart_click = true;
        uiController.restart_bool = true;
    }

    public void OnChangeLevelClick()
    {
        change_level_click = true;
        uiController.change_level_bool = true;
    }

    public void OnResetClick()
    {
        reset_click = true;
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }
}
