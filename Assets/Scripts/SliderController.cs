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
        OnDifficultyChange();
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
        UpdateText();

        changed_difficulty = true;
    }

    public int GetDifficultyValue()
    {
        return (int)GetComponent<Slider>().value;
    }

    public void SetDifficultyBaseText(string b_text)
    {
        base_text = b_text;
        UpdateText();
    }
    public void UpdateText()
    {
        value_text.text = base_text + toAdd;
    }
}
