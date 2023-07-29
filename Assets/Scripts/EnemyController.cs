using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // ASSIGNATIONS
    private Renderer objectRenderer;
    private GameManager gameManager;
    private GameObject[] targets;
    private GameObject target;
    private GameObject belonging_uienemy;
    public bool entered_vision;
    public GameObject explosion;
    public Vector3 default_scale;
    private bool instantiation = true; // to check if instantiation already happened and we are only modifying parameters

    // PARAMETERS BASELINE (WILL BE MULTIPLIED AND ACTUATED IN THE SETTING PARAMETERS FUNCTION
    private float health = 1;
    public float speed = 1;
    private float enemySize = 1;
    
    // BOUNDERS
    public bool insidebound = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        objectRenderer = GetComponent<Renderer>();

        // If the Renderer component doesn't exist in the current object, then search in its children
        if (objectRenderer == null)
        {
            objectRenderer = GetComponentInChildren<Renderer>();
        }

        if (objectRenderer.IsVisibleFrom(Camera.main))
            { // If it spawned inside the vision
                gameObject.tag = "Enemy";
                entered_vision = true;
            } else
            { // If it spawned outside of vision (please make sure the enemy will actually pass inside the camera vision)
                gameObject.tag = "OutboundEnemy";
                entered_vision = false;
            }
    }

    // Update is called once per frame
    void Update()
    {
        targets = GameObject.FindGameObjectsWithTag("EnemiesTarget");
            for (int i = 0; i < targets.Length; i++)
                if (targets[i].name == gameObject.name.Substring(0, 2)) // The name in game will be for example "e1(Clone)". The actual enemy name is only the first two char.
                {
                    target = targets[i];
                }
        
        if (gameObject.name.Substring(0, 2) == "e2") // One of the enemies goes against the player
            target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

            if (objectRenderer.IsVisibleFrom(Camera.main))
            {
                gameObject.tag = "Enemy";
                entered_vision = true;
            } else
            {
                if (entered_vision && gameObject.tag == "Enemy") // It entered vision and then went out
                {
                    gameObject.tag = "SavedEnemy";
                    gameManager.ImSave(gameObject.name, gameObject.tag);                    
                }
            }

            // DEATH
            if (health <= 0)
            {
                Instantiate(explosion, transform.position, Quaternion.identity);
                Die("dunno");
            }
        }
        else
        {
            SetEnemyParams();

            GameObject[] uienemies = GameObject.FindGameObjectsWithTag("GUIEnemy");
            for (int i = 0; i < uienemies.Length; i++)
                if (uienemies[i].name == gameObject.name.Substring(0, 2)) // The name in game will be for example "e1(Clone)". The actual enemy name is only the first two char.
                {
                    belonging_uienemy = uienemies[i];
                }
            
            if (belonging_uienemy != null)
                transform.position = Vector2.MoveTowards(transform.position, belonging_uienemy.transform.position, speed * Time.deltaTime);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (gameObject.tag == "Enemy") // If enemy is inside bounds and game is not finished (SavedEnemy tag)
        {
            if (other.gameObject.CompareTag("Bullet"))
            {
                //Debug.Log("Hit by bullet!");
                Instantiate(explosion, transform.position, Quaternion.identity);
                other.gameObject.tag = "Untagged";
                health -= 1;
                Destroy(other.gameObject);
                if (health <= 0)
                {
                    Die("bullet");
                }
            }

            if (other.gameObject.CompareTag("Player"))
            {
                return; // this is handled in the player's collider                            
            }
        }        
    }

    public void SetEnemyParams(float en_health = 1, float en_speed = 1.5f, float en_size = 1)  // (Default = 1, 1.5, 1)
    {
        if (instantiation) // If it's the first time we are setting them
        {
            default_scale = transform.localScale;
            instantiation = false;
        }

        health = en_health;

        speed = en_speed;

        enemySize = en_size;
        transform.localScale = default_scale * enemySize;
    }

    public float GetSpeed()
    {
        return speed;
    }
    public void SetSpeed(float en_speed)
    {
        speed = en_speed;
    }

    public void Die(string how)
    {
        gameManager.IDied(gameObject, gameObject.name, gameObject.tag, how);
        gameObject.tag = "DeadEnemy";
    }
}
