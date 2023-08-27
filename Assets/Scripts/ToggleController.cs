using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ToggleController : MonoBehaviour
{
    public TextMeshProUGUI value_text;

    // Start is called before the first frame update
    void Start()
    {
        value_text.text = "False";
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangedValue()
    {
        if(gameObject.GetComponent<Toggle>().isOn)
        {
            value_text.text = "True";
        }
        else
        {
            value_text.text = "False";
        }
    }
}
