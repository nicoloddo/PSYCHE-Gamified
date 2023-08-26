using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroductionController : MonoBehaviour
{
    private ChoiceController cc;
    private GameManager gameManager;

    public GameObject first_panel;
    public GameObject next_panel;
    public GameObject std_canva;

    // Start is called before the first frame update
    void Start()
    {
        cc = FindObjectOfType<ChoiceController>();
        gameManager = FindObjectOfType<GameManager>();
        

        Time.timeScale = 0;

        if(first_panel == null)
        {
            first_panel = next_panel;
        }
           
        next_panel.SetActive(false);
        std_canva.SetActive(false);

        first_panel.SetActive(true); // It needs to be last to handle the case of first_panel = next_panel
    }

    // Update is called once per frame
    void Update()
    {
        if(first_panel.activeSelf || next_panel.activeSelf)
            gameManager.introCursorActive = true;
        else
            gameManager.introCursorActive = false;
    }

    public void InformationSheetShow()
    {
        Application.ExternalEval("togglePDF(true);"); // to show the PDF
    }

    public void InformationSheetHide()
    {
        Application.ExternalEval("togglePDF(false);"); // to hide the PDF
    }

    public void InformationSheetRedirect()
    {
        string directory = "/directory";
        string jsCode = $"window.open(window.location.protocol + '//' + window.location.hostname + '{directory}', '_self');";
        Application.ExternalEval(jsCode);
    }

    public void OnAcceptClick()
    {
        next_panel.SetActive(true);
        first_panel.SetActive(false);
        cc.SendAccepted();
    }

    public void OnStartClick()
    {
        gameManager.introCursorActive = false;

        std_canva.SetActive(true);

        // The following objects might contain this object
        first_panel.SetActive(false);
        next_panel.SetActive(false);        
    }
}
