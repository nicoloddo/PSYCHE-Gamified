using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask playerLayer;
    private float timeLife = 7f; // Time life of bullet
    public float checkHitDistance = 20f; // Distance from the player to check for a soon collision
    private float timerLife; // Timer for calculate current time life of bullet
    private bool hit_checked = false;

    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {

    }

    void FixedUpdate()
    {    
        // Always point forward
        velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        if (velocity != Vector2.zero)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }        
        

        timerLife += Time.deltaTime; // Timer to autodestruct bullet

        if (timerLife >= timeLife) // If timer is ended
        {
            Destroy(gameObject);
        }


        // Cast a ray from the bullet in its direction of travel
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, checkHitDistance, playerLayer);

        if (hit.collider != null)
        {
            // Draw a red line from the start point to the point of impact
            Debug.DrawRay(transform.position, transform.right * hit.distance, Color.blue);
            AIController AI = hit.collider.gameObject.GetComponent<AIController>();
            PlayerController Pl = hit.collider.gameObject.GetComponent<PlayerController>();

            bool immune = timerLife < Pl.immunity_time / Time.timeScale;

            if(!immune && !hit_checked)
            {
                Debug.Log("Bullet hit incoming! Slowing time...");
                AI.BulletTowardsPlayer();
                hit_checked = true;                
            }
        }
        else
        {
            // If no hit was detected, draw a green line showing the full check distance
            Debug.DrawRay(transform.position, transform.right * checkHitDistance, Color.green);
        }
    }

    public float GetLife()
    {
        return timerLife;
    }
}

