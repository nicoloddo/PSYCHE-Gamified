using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SliderController : MonoBehaviour
{
    public TextMeshProUGUI value_text;
    public string rounding = "F1";
    private string base_text = "";
    private string toAdd = "";

    public bool changed_difficulty = false; // to communicate with UIController

    // Start is called before the first frame update
    void Start()
    {
        changed_difficulty = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDifficultyChange()
    {
        switch((int)GetComponent<Slider>().value)
        {
            case 1:
                toAdd = "Easy";
                break;
            case 2:
                toAdd = "Medium";
                break;
            case 3:
                toAdd = "Hard";
                break;
        }
        UpdateDifficultyText();

        changed_difficulty = true;
    }

    public void OnMOSChange()
    {
        string label;

        switch((int)GetComponent<Slider>().value)
        {
            case 1:
                label = "1 - Bad";
                break;
            case 2:
                label = "2 - Poor";
                break;
            case 3:
                label = "3 - Fair";
                break;
            case 4:
                label = "4 - Good";
                break;
            case 5:
                label = "5 - Excellent";
                break;
            default:
                label = "";
                break;
        }
        UpdateText(label);
    }

    public int GetDifficultyValue()
    {
        return (int)GetComponent<Slider>().value;
    }

    public int GetMOSValue()
    {
        return (int)GetComponent<Slider>().value;
    }

    public void SetDifficultyBaseText(string b_text)
    {
        base_text = b_text;
        UpdateDifficultyText();
    }

    public void UpdateText(string label)
    {
        value_text.text = label;
    }

    public void UpdateDifficultyText()
    {
        value_text.text = base_text + toAdd;
    }


}
