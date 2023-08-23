using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // ASSIGNMENTS
    private Animator animator;
    //private CharacterController characterController;
    private Rigidbody2D rigidBody;
    private GameManager gameManager;
    private SoundManager soundManager;
    public Animator engine_animator;
    public GameObject explosion;
    public GameObject explosion_shield;
    public GameObject bullet;
    private GameObject target;
    private AIController AI;

    // INPUTS
    private float inputRotation;
    private float inputVertical;
    private float inputHorizontal;

    // AUX
    public float totalRotation = 0;
    public int last_dir = 1; // 1 = right, -1 = left
    private float movX, movY;
    private float speedForceAmount;

    // PARAMETERS
    private float health;
    private float rotationSpeed = 5f;
    private float playerSpeed = 5f;
    private float forceMultiplier = 100f;
    public float shootDelay = 0.5f;
    public float aimDelay = 1f;
    public float delaysTimestep = 0.1f;
    public float accumulatedTime = 0;
    public float shootSlowRate = 1;
    public float launchForce = 15f;
    public float autoaimSpeed = 5f;
    public int bulletsToShoot = 1;
    public float enemyDamage = 1;
    public float bulletDamage = 1;
    private bool freeShooting = false;
    private bool slowingTime = false;
    private float bullet_offset = 0.5f; // offset to spawn the bullet where the weapon is
    public float immunity_time = 0.5f; // how many seconds the player is immune to the bullet they just shot

    // BOUNDERS
    private float maxRotation = 75;

    // AIMS
    private Vector2 targetDirection;
    private Vector2 aim;
    public bool aimed = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
        animator = GetComponent<Animator>();
        //characterController = GetComponent<CharacterController>();      
        rigidBody = GetComponent<Rigidbody2D>();     

        speedForceAmount = playerSpeed * forceMultiplier;

        AI = gameObject.GetComponent<AIController>();
    }

    // Update is called once per frame
    void Update()
    {
        inputVertical = Input.GetAxis("Vertical");
        inputHorizontal = Input.GetAxis("Horizontal");
        inputRotation = Input.GetAxis("Rotation");

        // SHOOTING
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(freeShooting)
            {
                Fire(bulletsToShoot);
            }
            else
            {
                AI.PlayerSlowTime();
            }
            
        }
    }

    private void FixedUpdate()
    {
        // MOVEMENT
        movX = inputHorizontal;
        movY = inputVertical;

        rigidBody.velocity = Vector2.zero; 
        rigidBody.angularVelocity = 0; 
        Vector2 force = new Vector2(movX, movY) * speedForceAmount;
        
        if(Time.timeScale < 1 && Time.timeScale > 0)
        {
            rigidBody.AddForce(force / (Time.timeScale*3)); // I divide to the timescale to keep the velocity constant regardless of the timescale
        }
        else
        {
            rigidBody.AddForce(force);
        }


        // Calculate and update new orientation
        if(last_dir < 0)
        {
            //inputRotation = -inputRotation;
        }
        totalRotation += last_dir*(inputRotation * rotationSpeed);

        if (totalRotation > maxRotation)
        {
            totalRotation = maxRotation;
        }
        else if (totalRotation < -maxRotation)
        {
            totalRotation = -maxRotation;
        }
        transform.rotation = Quaternion.Euler(0f, 0f, totalRotation);

        // AIM
        if (last_dir == 1)
        {
            aim = transform.right;
        }
        else if (last_dir == -1)
        {
            aim = -transform.right;
        }

        // AUTODIRECTION
        if (target == null)
        {
            target = AI.FindCloseDistance();
        } else
        {
            if (! target.gameObject.CompareTag("Enemy")) // if the tag is not enemy anymore
            {
                target = null;
                aimed = false;
            } else {               

                Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.red);
                targetDirection = (target.transform.position - transform.position).normalized;

                if (targetDirection.x * last_dir < 0) // If target is on the other direction
                {
                    transform.localScale = Vector3.Scale(transform.localScale, new Vector3(-1, 1, 1)); // Pointwise product just to flip the x of localScale
                    totalRotation *= -1;
                    last_dir *= -1;
                }

                // AUTOAIM AND FIRE
                if (aimed == false)
                {
                    if (targetDirection.y - aim.y > 0)
                    {
                        totalRotation += last_dir * (0.5f * autoaimSpeed);
                    }
                    else if (targetDirection.y - aim.y < 0)
                    {
                        totalRotation += last_dir * (-0.5f * autoaimSpeed);
                    }

                    if (Mathf.Abs(targetDirection.y - aim.y) < 0.1)
                    {
                        aimed = true;
                        StartCoroutine(wait_and_fire(shootDelay, aimDelay, bulletsToShoot));                        
                    }
                }
            }
        }
               

        // DEATH
        if (health <= 0)
        {
            gameManager.IDied(gameObject, gameObject.name, gameObject.tag, "zero_health");
            Instantiate(explosion, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        

    }

    public void SetParams(float playerHealth, float playerShootDelay, float playerAimDelay, float playerLaunchForce, float playerAutoaimSpeed, int playerBulletsToShoot, bool playerFreeShooting, bool playerSlowTime)
    {
        health = playerHealth;
        shootDelay = playerShootDelay;
        aimDelay = playerAimDelay;
        launchForce = playerLaunchForce;
        autoaimSpeed = playerAutoaimSpeed;
        bulletsToShoot = playerBulletsToShoot;
        freeShooting = playerFreeShooting;
        slowingTime = playerSlowTime;
    }

    public void SlowDownPlayer(float rate, float shoot_slowrate, float speed_slowrate)
    {
        float speed = GetSpeed() * speed_slowrate;
        shootSlowRate *= shoot_slowrate;
        float launchForce = GetLaunchForce() * shoot_slowrate;
        float autoaimSpeed = GetAutoaimSpeed() * shoot_slowrate;
        SetTimeParams(speed, launchForce, autoaimSpeed);
        gameObject.GetComponent<Animator>().speed *= speed_slowrate;
    }

    public void SetTimeParams(float speed, float playerLaunchForce, float playerAutoaimSpeed)
    {
        playerSpeed = speed;
        speedForceAmount = playerSpeed * forceMultiplier;
        launchForce = playerLaunchForce;
        autoaimSpeed = playerAutoaimSpeed;
    }

    public float GetSpeed()
    {
        return playerSpeed;
    }
    public float GetShootDelay()
    {
        return shootDelay;
    }
    public float GetAimDelay()
    {
        return aimDelay;
    }
    public float GetLaunchForce()
    {
        return launchForce;
    }
    public float GetAutoaimSpeed()
    {
        return autoaimSpeed;
    }

    public float GetHealth()
    {
        return health;
    }

    /*
    public GameObject FindCloseDistance()
    {
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
    */

    private void Fire(int amount)
    {
        animator.Play("shoot", 0, 0.25f);
        soundManager.PlayShoot();
        GameObject bullet_obj;
        Vector2 spawnPosition = transform.position + bullet_offset * transform.right;

        if(amount > 0)
        {
            bullet_obj = Instantiate(bullet, spawnPosition, transform.rotation);
            for (int i=1; i < amount-1; i++)
                bullet_obj = Instantiate(bullet, spawnPosition, transform.rotation);
                bullet_obj.GetComponent<Rigidbody2D>().AddForce(aim * launchForce, ForceMode2D.Impulse);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        float damage = 0;

        if(gameManager.GetWonOrLost() == 0) // while still playing
        {
            if (other.gameObject.CompareTag("BulletPlayerDamage"))
            {
                // the collider of the projectile is set to not collide with the player to not create uncontrolled forces on the player
                // instead, a child object of the projectile handles the collisions with the player, therefore the collider is of a child of the bullet
                GameObject bullet = other.transform.parent.gameObject;
                if (bullet.GetComponent<Bullet>().GetLife() < immunity_time) // if the bullet exists since less than 1 second
                {
                    return;
                }
                else
                {
                    other.gameObject.tag = "Untagged";
                    damage = bulletDamage;
                    Destroy(bullet);                 
                }
            }

            if (other.gameObject.CompareTag("Enemy"))
            {
                damage = enemyDamage;
                other.gameObject.GetComponent<EnemyController>().Die("player_contact");
            }

            if (damage > 0)
            {
                if((health == 1 || health == 3) && AI.active_shield) // final explosion is a shield
                {
                    AI.ShieldConversation();
                    AI.active_shield = false;
                    Instantiate(explosion_shield, transform.position, Quaternion.identity);
                }
                else
                {
                    health -= damage;
                    Instantiate(explosion, transform.position, Quaternion.identity);
                    soundManager.PlayLostLife();
                }
            }            
        }        
    }

    IEnumerator wait_and_fire(float shoot_delay, float aim_delay, int amount)
    {
        accumulatedTime = 0;

        // Wait for shoot delay
        while (accumulatedTime < shoot_delay / shootSlowRate)
        {
            yield return new WaitForSeconds(delaysTimestep);
            accumulatedTime += delaysTimestep;
        }
        
        Fire(amount);
        accumulatedTime = 0; // Reset accumulated time

        // Wait for aim delay
        while (accumulatedTime < aim_delay / shootSlowRate)
        {
            yield return new WaitForSeconds(delaysTimestep);
            accumulatedTime += delaysTimestep;
        }
        
        aimed = false;
    }


}
