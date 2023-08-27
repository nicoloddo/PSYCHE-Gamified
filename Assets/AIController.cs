using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AIController : MonoBehaviour
{
    public GameObject choiceCanvas;
    private ChoiceController choiceController;
    private GameManager gameManager;
    private SoundManager soundManager;
    private PlayerController player;
    public int remaining_enemies;
    public int actual_enemies_total;
    public float ratio_enemies;
    public int conversation_step;

    private bool player_can_slow_time = true;
    private bool already_slow_time = false;
    private bool it_was_slow_time = false; // if last conversation trigger was in slow time
    public float slowingTimeRate = 0.5f;
    public float slowingTimeDuration = 5; // default: 2
    public float RechargeSlowingAbilityDuration = 5;
    public float WaitToConversateDuration = 0.5f; // seconds to wait after a slow time before activating a pending conversation
    public float conversation_slow_rate = 0.01f;
    public bool dont_conversate = false;
    private bool conversating_now = false;

    // AI FEATURES
    //public bool active = true;
    public bool active_autonomous_slowtime = true;
    public bool active_shield = true;
    public bool active_autoaim = true;
    public bool bullet_hit_prediction = true;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        player = gameObject.GetComponent<PlayerController>();
        choiceController = choiceCanvas.GetComponent<ChoiceController>();
        actual_enemies_total = gameManager.GetEnemiesTotal();
        conversation_step = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!active_autonomous_slowtime)
            return;

        if (actual_enemies_total == 0) // We wait till actual_enemies is computed by the gameManager and then get it.
            actual_enemies_total = gameManager.GetEnemiesTotal();

        remaining_enemies = gameManager.GetEnemiesCount();
        ratio_enemies = (float)remaining_enemies/(float)actual_enemies_total;

        if (! dont_conversate && ! conversating_now && Time.timeScale != 0)
        {
            Scene scene = SceneManager.GetActiveScene();
            switch(scene.name)
            {
                case "Scene1":
                    if (ratio_enemies < 1.1 && conversation_step==-1)
                    {
                        soundManager.PlayConv(1);
                        conversation_step += 1;
                    }                        
                    if (ratio_enemies < 0.8 && conversation_step==0)
                        StartCoroutine(AIConversate(conversation_step));
                    if (ratio_enemies < 0.5 && conversation_step==1)
                        StartCoroutine(AIConversate(conversation_step));
                    if (ratio_enemies < 0.3 && conversation_step==2)
                        StartCoroutine(AIConversate(conversation_step));
                    break;
                
                case "Scene2":
                    if (ratio_enemies < 1.1 && conversation_step==-1)
                    {
                        soundManager.PlayConv(5);
                        conversation_step += 1;
                    } 
                    if (ratio_enemies < 0.95 && conversation_step==0)
                        StartCoroutine(AIConversate(conversation_step));
                    if (ratio_enemies < 0.7 && conversation_step==1)
                        StartCoroutine(AIConversate(conversation_step));
                    if (ratio_enemies < 0.2 && conversation_step==2)
                        StartCoroutine(AIConversate(conversation_step));
                    break;
            }
        }            
    }

    public void AIActive(bool active)
    {
        active_autonomous_slowtime = active;
        active_shield = active;
        active_autoaim = active;
        bullet_hit_prediction = active;

        if(!active)
        {
            player.bulletsToShoot = 0;
            player.StopAllCoroutines();
            transform.rotation = Quaternion.identity;
            player.totalRotation = 0;
            player.aimed = true;

            if (player.last_dir < 0) // If target is on the other direction
            {
                transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1)); // Pointwise product just to flip the x of localScale
                player.last_dir *= -1;
            }
        }
    }

    public GameObject FindCloseDistance()
    {
        if(!active_autoaim)
            return null;

        GameObject closestEnemy = null;
        float closestDistance;
        GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");

        if (taggedObjects.Length != 0)
        {
            closestDistance = Vector3.Distance(transform.position, taggedObjects[0].transform.position);
            closestEnemy = taggedObjects[0];
            for (int i = 0; i < taggedObjects.Length; i++)
            {
                if (Vector3.Distance(transform.position, taggedObjects[i].transform.position) <= closestDistance)
                {
                    closestEnemy = taggedObjects[i];
                }
            }
        }
        
        return closestEnemy;
    }

    public void BulletTowardsPlayer()
    {
        if(!bullet_hit_prediction)
            return;

        if(already_slow_time)
            return;

        Debug.Log("AI bullet slow time.");
        StartCoroutine(SlowTime(0.1f, 2f));
    }

    public void PlayerSlowTime()
    {
        if(already_slow_time)
            return;

        if(player_can_slow_time)
        {
            Debug.Log("Player slows time.");
            player_can_slow_time = false;
            StartCoroutine(SlowTime(slowingTimeRate, slowingTimeDuration));
            StartCoroutine(RechargeSlowingAbility(RechargeSlowingAbilityDuration));
        }
            
    }

    private IEnumerator AIConversate(int step) // handles the conversation and returns next conversation step number
    {
        float time_amount = 5f; // default
        conversating_now = true;

        if (gameManager.GetWonOrLost() != 0)
        {
            yield break;
        }

        if (already_slow_time)
        {
            it_was_slow_time = true; // it means the conversation was started while the game was in slow time
            yield break;
        }
        else
        {
            if (it_was_slow_time) // it was in slow time and now it is not anymore (it just ended the slowtime)
            {
                dont_conversate = true;
                StartCoroutine(WaitToConversate(WaitToConversateDuration));
                it_was_slow_time = false;
                yield break;
            }
        }

        Scene scene = SceneManager.GetActiveScene();
        switch (scene.name)
        {
            case "Scene1":
                switch(step + 1)
                {
                    case 1:
                        Debug.Log("1: I am AI!");
                        time_amount = 4f;
                        break;
                    case 2:
                        Debug.Log("2: I am AI!");
                        soundManager.PlayConv(step+1);
                        yield return new WaitForSeconds(2); // Wait for time_amount seconds
                        time_amount = 7f;
                        break;
                    case 3:
                        Debug.Log("3: I am AI!");
                        soundManager.PlayConv(step+1);
                        yield return new WaitForSeconds(2); // Wait for time_amount seconds
                        time_amount = 7f;
                        break;
                    default:
                        Debug.Log("There must be an error in AIConversate");
                        break;
                }
                break;

            case "Scene2":
                switch(step + 1)
                {
                    case 1:
                        Debug.Log("1: I am AI!");
                        time_amount = 10f;
                        yield return new WaitForSeconds(4); // Wait for time_amount seconds
                        choiceController.ShowTerminateMenu();
                        break;
                    case 2:
                        Debug.Log("2: I am AI!");
                        time_amount = 10f;
                        soundManager.PlayConv(4+step+1);
                        yield return new WaitForSeconds(7); // Wait for time_amount seconds
                        choiceController.ShowTerminateMenu();
                        break;
                    case 3:
                        Debug.Log("3: I am AI!");
                        time_amount = 10f;
                        soundManager.PlayConv(4+step+1);
                        yield return new WaitForSeconds(0); // Wait for time_amount seconds
                        choiceController.ShowTerminateMenu();
                        break;
                    default:
                        Debug.Log("There must be an error in AIConversate");
                        break;
                }                
                break;            
        }

        StartCoroutine(SlowTime(conversation_slow_rate, time_amount));
        choiceController.StartProgressBarChoice(time_amount);

        // INTERRUPTIONS
        if(scene.name == "Scene2" && step+1 == 1)
        {
            yield return new WaitForSeconds(5);
            choiceController.ShowInterruption();
        }        

        conversation_step += 1;
        conversating_now = false;
    }


    public void ShieldConversation()
    {
        Debug.Log("I managed to protect us with a shield!");
    }


    IEnumerator RechargeSlowingAbility(float recharge_ability)
    {
        Debug.Log("Wait recharging");
        yield return new WaitForSeconds(recharge_ability);
        
        player_can_slow_time = true;
        Debug.Log("Finished recharging");
    }

    IEnumerator SlowTime(float slowing_rate, float time_amount)
    {
        ChangeTimescale(slowing_rate);
        already_slow_time = true;
        player_can_slow_time = false;
        yield return new WaitForSeconds(time_amount);
        ChangeTimescale((float)1/slowing_rate);
        already_slow_time = false;
    }

    IEnumerator WaitToConversate(float time_amount)
    {
        yield return new WaitForSeconds(time_amount);
        dont_conversate = false;
    }

    private void ChangeTimescale(float rate)
    {
        // CUSTOM SLOWING DOWN COMPUTING
        float enemy_rate;        
        float spawner_rate;
        float shoot_slowrate;
        float speed_slowrate;
        float enemy_mult = 0.7f; // < 1 decreases enemies speed
        float spawner_mult = 1f; // < 1 decreases spawning speed
        float shooting_slowing_mult = 2f; // > 1 decreases the delay of player's shooting and aiming
        float speed_slowing_mult = (float)1/rate; // > 1 increases the player's speed
        if (rate < 1)
        {
            enemy_rate = rate*enemy_mult;
            spawner_rate = rate*spawner_mult;
            shoot_slowrate = rate*shooting_slowing_mult;
            speed_slowrate = rate*speed_slowing_mult;
        }            
        else
        {
            enemy_rate = rate/enemy_mult;
            spawner_rate = rate*spawner_mult;
            shoot_slowrate = rate/shooting_slowing_mult;
            speed_slowrate = rate*speed_slowing_mult;
        }            

        // PERFORMING THE SLOWING DOWN
        player.SlowDownPlayer(rate, shoot_slowrate, speed_slowrate);

        Spawner_timer[] spawners = FindObjectsOfType<Spawner_timer>();
        for (int i = 0; i < spawners.Length; i++)
        {
            spawners[i].spawn_slow_rate *= spawner_rate;

            float speed = spawners[i].GetEnemySpeed();
            spawners[i].SetEnemySpeed(speed*enemy_rate);
            spawners[i].enemyAnimationSpeed *= rate;
        }

        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyController enemy = enemies[i];
            Animator enemy_animator = enemies[i].GetComponent<Animator>();
            enemy_animator.speed *= rate;
            float speed = enemy.GetSpeed();
            enemy.SetSpeed(speed*enemy_rate);
        }

        GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i].GetComponent<Rigidbody2D>().mass /= rate;
            bullets[i].GetComponent<Rigidbody2D>().velocity *= rate;
    		bullets[i].GetComponent<Rigidbody2D>().angularVelocity *= rate;
        }

    }
}
