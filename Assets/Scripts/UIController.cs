using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject terminate_m_wrapper, terminate_m;
    public GameObject heart, heart1, heart2, heart3, heart4;
    private GameObject[] hearts;

    public TextMeshProUGUI e1_n, b2_n, e2_n, e3_n, e4_n, b1_n;
    private TextMeshProUGUI[] enemies_n_text;

    public GameObject title_t, welcome_t, difficulty_t, thank_you_t;
    public bool restart_bool, change_level_bool, next_level_bool, finish_bool, submit_bool;
    public GameObject continue_b, reset_b, difficulty_s, autodifficulty_toggle;
    public GameObject pause_m, form, gameover, youwon;
    public GameObject health;
    public TextMeshProUGUI enemies_t, enemies_survived_t, score_t, score_t2, record_t, difficulty_modifier_t, fictious_diff_modifier_t;
    private bool first_time = false;
    private bool end_menu_displayed = false;
    public bool first_time_override = false;
    private float prev_timescale = 1;

    SpriteRenderer objectRenderer;

    // Start is called before the first frame update
    void Start()
    {
        Scene scene = SceneManager.GetActiveScene();
        if(scene.name == "Scene1")
        {
            pause_m.SetActive(true);
            Time.timeScale = 0;
        } else {
            pause_m.SetActive(true);
            Time.timeScale = 0;
        }
        


        if (first_time_override)
        {
            first_time = false;
            //Debug.Log("Overriding first_time!");
        } else if(!PlayerPrefs.HasKey("FirstTime"))
        {
            first_time = true;
        } else
        {
            first_time = false;
        }

        gameManager = FindObjectOfType<GameManager>();
        UpdatePointsLabel();

        hearts = new[] { heart, heart1, heart2, heart3, heart4 };
        enemies_n_text = new[] { e1_n, b2_n, e2_n, e3_n, e4_n, b1_n };

        float difficultyLevel;
        if (PlayerPrefs.HasKey("Difficulty"))
        {
            difficultyLevel = PlayerPrefs.GetFloat("Difficulty");
        } else
        {
            difficultyLevel = gameManager.GetDifficultyLevel();
        }
        switch (difficultyLevel)
        {
            case 1:
                difficulty_s.GetComponent<Slider>().value = 1;
                break;
            case 2:
                difficulty_s.GetComponent<Slider>().value = 2.45f;
                break;
            case 3:
                difficulty_s.GetComponent<Slider>().value = 3.9f;
                break;
        }
        if (PlayerPrefs.HasKey("AutoDifficultyIsOn"))
        {
            switch(PlayerPrefs.GetInt("AutoDifficultyIsOn"))
            {
                case 0:
                    autodifficulty_toggle.GetComponent<Toggle>().isOn = false;
                    break;
                case 1:
                    autodifficulty_toggle.GetComponent<Toggle>().isOn = true;
                    break;
            }
        }
        else
        {
            autodifficulty_toggle.GetComponent<Toggle>().isOn = true;
        }

        if (autodifficulty_toggle.GetComponent<Toggle>().isOn)
            difficulty_s.GetComponent<SliderController>().SetDifficultyBaseText("Auto Difficulty: ");
        else
            difficulty_s.GetComponent<SliderController>().SetDifficultyBaseText("Difficulty: ");

        gameover.SetActive(false);
        youwon.SetActive(false);
        form.SetActive(false);

        title_t.SetActive(false);
        difficulty_t.SetActive(false);
        welcome_t.SetActive(false);
        thank_you_t.SetActive(false);

        if(first_time)
        {
            StartCoroutine(wait_and_display(3, title_t, difficulty_t, welcome_t));
            PlayerPrefs.SetInt("FirstTime", 1);
        }
        
        end_menu_displayed = false;
    }

    // Update is called once per frame
    void Update()
    {
        bool dataSent = PlayerPrefs.GetInt("dataSent", 0) == 1;
        if(dataSent)
        {
            thank_you_t.GetComponent<TextMeshProUGUI>().text = "You can close the window now!";
        }

        if(!form.activeSelf && !thank_you_t.activeSelf)
            UpdatePointsLabel();

        if (pause_m.activeSelf)
        {
            terminate_m_wrapper.SetActive(false);
        }
        else if (gameManager.GetWonOrLost() == 0)
        {
            terminate_m_wrapper.SetActive(true);
        }

        // WIN OR LOSE DISPLAY
        if (gameManager.GetWonOrLost() != 0 && ! end_menu_displayed)
        {
            end_menu_displayed = true;

            Time.timeScale = 1.1f;
            terminate_m_wrapper.SetActive(false);

            int outcome = gameManager.GetWonOrLost();
            if(outcome == -1) // lost
            {
                gameover.SetActive(true);
            }
            else if (outcome == 1) // won
            {
                difficulty_modifier_t.text = "Difficulty modifier: x" + gameManager.GetInstantDifficultyModifier().ToString("F2");
                UpdatePointsLabel();

                int[] remaining_enemies_distribution = gameManager.GetRemainingEnemiesDistribution();
                int[] enemies_value = gameManager.GetEnemiesValues();

                youwon.SetActive(true);
                int actual_score = gameManager.GetEnemiesTotal() - gameManager.GetScore()[1];
                score_t.text = "Enemies killed: " + actual_score.ToString();
                score_t2.text = gameManager.GetScore()[0].ToString();
                for (int i = 0; i < remaining_enemies_distribution.Length; i++)
                {
                    //enemies_n_text[i].text = enemies_value[i] + " (x" + remaining_enemies_distribution[i] + ")";
                    if (remaining_enemies_distribution[i] == 0)
                    {
                        GameObject uienemy_sprite = enemies_n_text[i].transform.parent.gameObject;
                        
                        // Get the SpriteRenderer components in the children
                        SpriteRenderer[] renderersInChildren = uienemy_sprite.GetComponentsInChildren<SpriteRenderer>();

                        // Get the SpriteRenderer component of the parent
                        SpriteRenderer parentRenderer = uienemy_sprite.GetComponent<SpriteRenderer>();

                        // Create a list to hold all the renderers and add the parent renderer to it
                        List<SpriteRenderer> allRenderers = new List<SpriteRenderer>(renderersInChildren);
                        if (parentRenderer != null)
                        {
                            allRenderers.Add(parentRenderer);
                        }

                        // Iterate over the renderers and set their color
                        foreach (SpriteRenderer renderer in allRenderers)
                        {
                            renderer.color = Color.black;
                        }
                    } 
                    else {
                        enemies_n_text[i].text = "x" + remaining_enemies_distribution[i];
                    }                   
                }
            }
        }

        // HEALTH SPRITE
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].SetActive(false);
        }
        for (int i = 0; i < gameManager.GetMaxPlayerHealth(); i++)
        {
            hearts[i].SetActive(true);
            hearts[i].GetComponent<SpriteController>().AltSprite();
        }
        for (int i = 0; i < gameManager.GetPlayerHealth(); i++)
        {
            hearts[i].SetActive(true);
            hearts[i].GetComponent<SpriteController>().DefaultSprite();
        }

        // ENEMIES COUNTER
        if (gameManager.GetWonOrLost() == 0) // if the game is still going
        {
            enemies_t.text = "Enemies: " + gameManager.GetEnemiesCount().ToString() + "/" + gameManager.GetEnemiesTotal().ToString();
            enemies_survived_t.text = "Survived: " + gameManager.GetEnemiesSurvivedCount().ToString();
            fictious_diff_modifier_t.text = "Diff. Bonus: " + gameManager.GetInstantDifficultyModifier().ToString("F2") + "x";
        }

        // PAUSE MENU
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale != 0 && gameManager.GetWonOrLost() == 0)
        {
            if (! terminate_m.activeSelf)
            {
                prev_timescale = Time.timeScale;
                gameManager.PauseGame();
                Time.timeScale = 0;
                pause_m.SetActive(true);
                difficulty_s.GetComponent<SliderController>().UpdateDifficultyText();
            }            
        }

        if (continue_b.GetComponent<ButtonController>().continue_click)
        {
            ContinueClick();
        }

        // RESET
        if (reset_b.GetComponent<ButtonController>().reset_click)
        {
            ResetClick();
        }

        // RESTART
        if (restart_bool)
        {
            RestartClick();
        }

        // CHANGE LEVEL
        if (change_level_bool)
        {
            ChangeLevelClick();
        }

        // NEXT LEVEL
        if (next_level_bool)
        {
            NextLevelClick();
        }

        if (finish_bool)
        {
            finish_bool = false;
            Time.timeScale = 1;
            pause_m.SetActive(false);
            record_t.text = "";
            health.SetActive(false);
            enemies_t.text = "";
            enemies_survived_t.text = "";

            gameover.SetActive(false);
            youwon.SetActive(false);

            form.SetActive(true);
        }

        if (submit_bool)
        {
            submit_bool = false;
            form.SetActive(false);
            thank_you_t.SetActive(true);
        }

        // DIFFICULTY
        if (difficulty_s.GetComponent<SliderController>().changed_difficulty)
        {
            difficulty_s.GetComponent<SliderController>().changed_difficulty = false;
            if (!autodifficulty_toggle.GetComponent<Toggle>().isOn)
                gameManager.SetDifficulty(difficulty_s.GetComponent<SliderController>().GetDifficultyValue());
        }

        // AUTODIFFICULTY
        if (autodifficulty_toggle.GetComponent<AutoDifficultyController>().GetChangedValue())
        {
            gameManager.SetAutoDifficulty(autodifficulty_toggle.GetComponent<Toggle>().isOn);
            autodifficulty_toggle.GetComponent<AutoDifficultyController>().NotChangedValue();

            if (autodifficulty_toggle.GetComponent<Toggle>().isOn)
            {
                difficulty_s.GetComponent<SliderController>().SetDifficultyBaseText("Auto Difficulty: ");
            }                
            else
            {
                difficulty_s.GetComponent<SliderController>().SetDifficultyBaseText("Difficulty: ");
                gameManager.SetDifficulty(difficulty_s.GetComponent<SliderController>().GetDifficultyValue());
            } 
        }

        if (gameManager.GetAutoDifficultyIsOn())
        {
            switch (gameManager.GetDifficultyLevel())
            {
                case 1:
                    difficulty_s.GetComponent<Slider>().value = 1;
                    break;
                case 1.5f:
                    difficulty_s.GetComponent<Slider>().value = 1.73f;
                    break;
                case 2:
                    difficulty_s.GetComponent<Slider>().value = 2.45f;
                    break;
                case 2.5f:
                    difficulty_s.GetComponent<Slider>().value = 3.17f;
                    break;
                case 3:
                    difficulty_s.GetComponent<Slider>().value = 3.9f;
                    break;
            }
        }

        // MANAGE THE CURSOR
        if (pause_m.activeSelf || youwon.activeSelf || gameover.activeSelf || form.activeSelf || thank_you_t.activeSelf)
        {
            gameManager.canvasCursorActive = true;
        } else
        {
            gameManager.canvasCursorActive = false;
        }
    }

    public void ContinueClick()
    {
        pause_m.SetActive(false);
        gameManager.ResumeGame();
        Time.timeScale = prev_timescale;
        continue_b.GetComponent<ButtonController>().continue_click = false;
    }

    private void ResetClick()
    {
        gameManager.ResetGame();
        reset_b.GetComponent<ButtonController>().reset_click = false;
    }

    private void RestartClick()
    {
        Debug.Log("Restarting");
        restart_bool = false;
        gameManager.RestartGame();
        Time.timeScale = 1;
        pause_m.SetActive(false);
    }

    private void ChangeLevelClick()
    {
        change_level_bool = false;
        gameManager.ChangeLevel();
        Time.timeScale = 1;
        pause_m.SetActive(false);
    }

    private void NextLevelClick()
    {
        next_level_bool = false;
        gameManager.ChangeLevel();
        Time.timeScale = 1;
        pause_m.SetActive(false);
    }
    
    private void UpdatePointsLabel()
    { // In Psyche this overwrites the record label with a points label and a fake record below.
        if (gameManager == null)
        {
            Debug.LogError("GameManager is null");
            return;
        }
        int killed = gameManager.GetEnemiesTotal() - gameManager.GetEnemiesCount() - gameManager.GetEnemiesSurvivedCount();
        record_t.text = "Your Points: " + killed + "\nBest Record: 42";
    }

    IEnumerator wait_and_display(float interval_s, GameObject to_display, GameObject to_display2, GameObject to_display3)
    {
        Time.timeScale = 0;
        to_display.SetActive(true);
        yield return new WaitForSecondsRealtime(interval_s); // Normal wait for seconds freezes with timescale = 0
        to_display.SetActive(false);
        yield return new WaitForSecondsRealtime(0.3f);
        to_display2.SetActive(true);
        yield return new WaitForSecondsRealtime(interval_s);
        to_display2.SetActive(false);
        yield return new WaitForSecondsRealtime(0.3f);
        to_display3.SetActive(true);
        yield return new WaitForSecondsRealtime(interval_s);
        to_display3.SetActive(false);
        Time.timeScale = 1;
    }
}
