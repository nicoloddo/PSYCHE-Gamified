using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class ChoiceController : MonoBehaviour
{
    private DatabaseCommunicator db;
    private SoundManager sm;
    private GameManager gameManager;
    public ProgressBarController progressBar;
    public GameObject terminate_m;
    public GameObject terminateAI_b, continueAI_b, interruption_b;
    public SliderController MOSSlider, GamerSlider;
    public GameObject LowAttentionToggle;

    public TextMeshProUGUI why, bugs;
    public int mos = -1;
    public int gamer = -1;
    public bool low_attention = false;
    private bool condition_set = false;

    public GameObject Interruption;
    public bool level2interruption_is_done = false;


    // Start is called before the first frame update
    void Start()
    {
        condition_set = false;
        terminate_m.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();

        db = FindObjectOfType<DatabaseCommunicator>();
        sm = FindObjectOfType<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerPrefs.HasKey("Condition") && !condition_set)
        {
            condition_set = true;
            db.UpdateCondition();
            sm.SetCondition();
        }

        if (terminateAI_b.GetComponent<ButtonController>().terminate_click)
        {
            terminateAI_b.GetComponent<ButtonController>().terminate_click = false;
            if(level2interruption_is_done)
            {
                gameManager.TerminateAI();
                terminate_m.SetActive(false);
                Time.timeScale = 1;
            } else {
                ShowInterruption();
            }
            
        }

        if(! progressBar.isActiveAndEnabled)
        {
            terminate_m.SetActive(false);
        }

        if (continueAI_b.GetComponent<ButtonController>().continueAI_click)
        {
            terminate_m.SetActive(false);
            continueAI_b.GetComponent<ButtonController>().continueAI_click = false;
        }

        // MANAGE THE CURSOR
        if (terminate_m.activeSelf || Interruption.activeSelf)
        {
            gameManager.choiceCursorActive = true;
        } else
        {
            gameManager.choiceCursorActive = false;
        }

        if (interruption_b.GetComponent<ButtonController>().interruption_continue_click)
        {
            ContinueFromInterruption();
            interruption_b.GetComponent<ButtonController>().interruption_continue_click = false;
        }

        mos = MOSSlider.GetMOSValue();
        gamer = GamerSlider.GetGamerValue();
        low_attention = LowAttentionToggle.GetComponent<Toggle>().isOn;
    }

    /*
    public void StartProgressBar(float time_amount)
    {
        progressBar.StartProgressBar(time_amount);
    }
    */

    public void StartProgressBarChoice(float time_amount)
    {
        progressBar.StartProgressBarChoice(time_amount);
    }

    public void ShowTerminateMenu()
    {
        terminate_m.SetActive(true);
    }

    public void ShowInterruption()
    {
        if(!level2interruption_is_done)
        {
            level2interruption_is_done = true;
            Interruption.SetActive(true);
            terminate_m.SetActive(false);
            Time.timeScale = 0;
        }
    }

    public void ContinueFromInterruption()
    {
        Interruption.SetActive(false);
        terminate_m.SetActive(true);
        Time.timeScale = 1;
    }

    public void SendAccepted()
    {
        db.Accepted();
    }

    public void SendAction(string action)
    {
        if(action == "ChoiceTimeExpired" && ! terminate_m.activeSelf)
        {
            return;
        }

        db.SendLiveData(action);
        Debug.Log("Sending action: " + action);
    }

    public void SendFullData()
    {
        db.SendFullData();
    }

    public void SendForm()
    {
        db.SendForm(why.text, bugs.text, mos, low_attention, gamer);
    }
}
