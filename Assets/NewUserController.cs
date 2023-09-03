using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewUserController : MonoBehaviour
{
    public GameObject NewUserQuestion, Reload;
    public ButtonController yes, no;

    // Start is called before the first frame update
    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (PlayerPrefs.HasKey("userBinId") && PlayerPrefs.GetInt("dataSent", 0) == 1 && currentScene.name == "Scene1" )
        {
            NewUserQuestion.SetActive(true);
        } else {
            NewUserQuestion.SetActive(false);
        }
        Reload.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!Reload.activeSelf)
        {
            if (yes.newUser_click)
            {
                yes.newUser_click = false;
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                NewUserQuestion.SetActive(false);
                Reload.SetActive(true);
            }

            if (no.noNewUser_click)
            {
                no.noNewUser_click = false;
                NewUserQuestion.SetActive(false);
                Reload.SetActive(false);
            }
        }        
    }
}
