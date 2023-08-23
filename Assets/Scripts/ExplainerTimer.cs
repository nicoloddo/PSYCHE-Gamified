using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExplainerTimer : MonoBehaviour
{
    private float timerLife; // Timer for calculate current time life
    public float timeLife = 1f; // Time life
    public GameObject toExplain;
    private TextMeshProUGUI ui_text;
    private string text;

    // Start is called before the first frame update
    void Start()
    {
        ui_text = gameObject.GetComponent<TextMeshProUGUI>();
        text = ui_text.text;
    }

    // Update is called once per frame
    void Update()
    {
        if(toExplain.activeSelf)
        {
            ui_text.text = text;
        }
        else
        {
            ui_text.text = "";
        }


        timerLife += Time.deltaTime; // Timer to autodestruct

        if (timerLife >= timeLife) // If timer is ended
        {
            Destroy(gameObject);
        }
    }
}
