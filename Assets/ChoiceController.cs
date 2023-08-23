using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChoiceController : MonoBehaviour
{
    private DatabaseCommunicator db;
    private GameManager gameManager;
    public ProgressBarController progressBar;
    public GameObject terminate_m;
    public GameObject terminateAI_b, continueAI_b;

    public TextMeshProUGUI why, bugs;


    // Start is called before the first frame update
    void Start()
    {
        terminate_m.SetActive(false);
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (terminateAI_b.GetComponent<ButtonController>().terminate_click)
        {
            gameManager.TerminateAI();
            terminateAI_b.GetComponent<ButtonController>().terminate_click = false;
            terminate_m.SetActive(false);
            Time.timeScale = 1;
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
        if (terminate_m.activeSelf)
        {
            gameManager.choiceCursorActive = true;
        } else
        {
            gameManager.choiceCursorActive = false;
        }
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

    public void SendAccepted()
    {
        db = FindObjectOfType<DatabaseCommunicator>();
        db.Accepted();
    }

    public void SendAction(string action)
    {
        Debug.Log("Sending action: " + action);
        db = FindObjectOfType<DatabaseCommunicator>();

        if(action == "ChoiceTimeExpired" && ! terminate_m.activeSelf)
        {
            return;
        }

        db.SendLiveData(action);
    }

    public void SendFullData()
    {
        db = FindObjectOfType<DatabaseCommunicator>();
        db.SendFullData();
    }

    public void SendForm()
    {
        db = FindObjectOfType<DatabaseCommunicator>();
        db.SendForm(why.text, bugs.text);
    }
}
