using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private static PlayerModel instance;

    public float playerskill = 0;
    public float average_diffmodifier = 0;

    public List<int> remaining_health_history;
    public List<int> score_history;
    public List<float> diffmodifier_history;
    public List<float> playerskill_history;
    public int n_sessions;
    public int recent_sessions_threshold = 2;

    private int max_health;
    private int max_score;
    public float suggestedLevel = -1;

    public int decrease_count;
    public int increase_count;

    void Awake()
    {
        // MANAGE THE DONTDESTROYONLOAD THING
        if (instance == null)
        {
            instance = this;
            gameObject.tag = "PlayerModel";
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        n_sessions = remaining_health_history.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMax(int max_h, int max_s)
    {
        max_health = max_h;
        max_score = max_s;
    }

    public void AddSessionData(int remaining_health, int score, float diff_modifier)
    {
        remaining_health_history.Add(remaining_health);
        score_history.Add(score);
        diffmodifier_history.Add(diff_modifier);
        n_sessions++;

        CalculatePlayerSkill();
        if(n_sessions >= recent_sessions_threshold)
            CheckPlayerSkill();
    }
    private void AddPlayerSkillHistory()
    {
        playerskill_history.Add(playerskill);
    }
    public void ResetSkillHistory()
    {
        playerskill = 0;
        playerskill_history.Clear();
    }
    public void ResetSessionData()
    {
        remaining_health_history.Clear();
        score_history.Clear();
        diffmodifier_history.Clear();
        n_sessions = 0;
        increase_count = 0;
        decrease_count = 0;
        AddPlayerSkillHistory();
    }

    private void CheckPlayerSkill()
    {
        // FLOW INTERVAL: 0.25, 0.75
        if (playerskill <= 0.25)
        {
            decrease_count++;
            increase_count = 0;

            if (playerskill <= 0.1) // BONUS
            {
                decrease_count++;
            }
        }
        if (playerskill >= 0.6)
        {
            increase_count++;
            decrease_count = 0;

            if (playerskill >= 0.7) // BONUS
            {
                increase_count++;
            }
        }
    }

    private void CalculatePlayerSkill() // (from 0 to 1)
    {
        float health_grade = 0;
        float score_grade = 0;

        float average_difficulty = 0;
        
        int sessions_threshold = (recent_sessions_threshold > n_sessions ? n_sessions : recent_sessions_threshold);
        float worst_health_grade = max_health * sessions_threshold;
        float max_score_grade = max_score * sessions_threshold;
        
        for (int i = 1; i <= sessions_threshold; i++)
        {
            health_grade += remaining_health_history[n_sessions - i];
            score_grade += score_history[n_sessions - i];
            average_difficulty += diffmodifier_history[n_sessions - i];
        }
        health_grade = 1 - (health_grade / worst_health_grade); // 25% of health grade means on average you remain with 75% health, health grade < 50% means you're bad bro
        score_grade /= max_score_grade; // 20% of the possible score is already quite enough to increase difficulty

        average_diffmodifier = average_difficulty / sessions_threshold;
        playerskill = (health_grade + score_grade) / 2; 
        // If you died, health grade will already be 100%, having 20% score grade will result in a 75% playerskill
        // If you survived, score grade will be 0%, having 50% health grade will result in having 25% playerskill
        // This is how I decided the thresholds to change difficulty
    }

    public float GetPlayerSkill()
    {
        return playerskill;
    }

    public int GetIncreaseCount()
    {
        return increase_count;
    }

    public int GetDecreaseCount()
    {
        return decrease_count;
    }

    public float GetSuggestedLevel()
    {
        return suggestedLevel;
    }

    public float GetAverageDiffModifier()
    {
        return average_diffmodifier;
    }

    public void SetSuggestedLevel(float level)
    {
        suggestedLevel = level;
    }
    public void SetSuggestedLevelIfNot(float level)
    {
        if(suggestedLevel == -1)
            suggestedLevel = level;
    }
}
