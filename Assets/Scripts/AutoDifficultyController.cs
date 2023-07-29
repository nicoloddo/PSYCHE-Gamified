using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoDifficultyController : MonoBehaviour
{
    public GameObject handle;
    private bool changed_value = false;

    // Start is called before the first frame update
    void Start()
    {
        changed_value = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<Toggle>().isOn)
        {
            handle.SetActive(false);
        }
        else
        {
            handle.SetActive(true);
        }
    }

    public void ChangedValue()
    {
        changed_value = true;
    }
    public bool GetChangedValue()
    {
        return changed_value;
    }
    public void NotChangedValue()
    {
        changed_value = false;
    }
    public void SetIsOn(bool isOn)
    {
        gameObject.GetComponent<Toggle>().isOn = isOn;
    }
}
