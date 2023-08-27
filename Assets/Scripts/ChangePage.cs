using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangePage : MonoBehaviour
{
    public ButtonController next_page_button;
    public GameObject next_page;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (next_page_button.next_page_click)
        {
            next_page.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
