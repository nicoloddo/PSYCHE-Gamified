using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private UIController uiController;
    private ChoiceController cc;
    public bool continue_click = false;
    public bool restart_click = false;
    public bool change_level_click = false;
    public bool next_level_click = false;
    public bool finish_click = false;  
    public bool submit_click = false;  
    public bool reset_click = false;
    public bool terminate_click = false;
    public bool continueAI_click = false;
    public bool interruption_continue_click = false;
    public bool next_page_click = false;
    public bool newUser_click = false;
    public bool noNewUser_click = false;

    // Start is called before the first frame update
    void Start()
    {
        uiController = FindObjectOfType<UIController>();
        cc = FindObjectOfType<ChoiceController>();

        continue_click = false;
        restart_click = false;
        change_level_click = false;
        next_level_click = false;
        finish_click = false;
        submit_click = false;
        reset_click = false;
        terminate_click = false;
        interruption_continue_click = false;
        next_page_click = false;
        newUser_click = false;
        noNewUser_click = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnContinueClick()
    {
        continue_click = true;
    }

    public void OnInterruptionContinueClick()
    {
        interruption_continue_click = true;
    }

    public void OnNextPageClick()
    {
        next_page_click = true;
    }

    public void OnNewUserYes()
    {
        newUser_click = true;
    }

    public void OnNewUserNo()
    {
        noNewUser_click = true;
    }

    public void OnInformationSheetShow()
    {
        Application.ExternalEval("togglePDF(true);"); // to show the PDF
    }

    public void OnTerminateAIClick()
    {
        terminate_click = true;
        cc.SendAction("AIterminated");
    }

    public void OnContinueAIClick()
    {
        continueAI_click = true;
        cc.SendAction("AIcontinue");
    }

    public void OnRestartClick()
    {
        restart_click = true;
        uiController.restart_bool = true;
        cc.SendAction("Restart");
    }

    public void OnChangeLevelClick()
    {
        change_level_click = true;
        uiController.change_level_bool = true;
    }

    public void OnNextLevelClick()
    {
        next_level_click = true;
        uiController.next_level_bool = true;
        cc.SendAction("NextLevel");
        cc.SendFullData();
    }

    public void OnFinishClick()
    {
        finish_click = true;
        uiController.finish_bool = true;
        cc.SendFullData();
    }

    public void OnSubmitClick()
    {
        submit_click = true;
        uiController.submit_bool = true;
        cc.SendForm();
    }

    public void OnResetClick()
    {
        reset_click = true;
    }

    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnAudioTestClick()
    {
        SoundManager sm = FindObjectOfType<SoundManager>();
        sm.StopAudioTest();
        sm.PlayAudioTest();
    }
}
