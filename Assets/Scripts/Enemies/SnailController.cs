/*  
-------------------------------------------  
SnailController.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/15/2024  
  
Purpose:  
This script defines the behavior for the "Crawler" enemy type.  
The Crawler patrols between ledges, flips direction upon reaching ledges or walls, and interacts with other enemies and the player.  
  
Key Features:  
Patrol logic with ledge and wall detection.  
State-based behavior transitions, including idle and flipping states.  
Collision handling with other enemies and the environment.  
Dynamic velocity adjustments for movement.  
  
Changelog:  
-------------------------------------------  
10/15/2024 - Initial version created. Added core patrol behavior and state logic for ledge and wall detection.  
10/16/2024 - Improved flip logic with timer-based delay and wall collision. Adjusted gravity scale for smoother movement.  
             Integrated health checks and death mechanics. Enhanced collision response for enemy interactions.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class SnailController : Enemy
{
    //Timer for flip delay
    private float timer;

    //Time the crawler waits before flipping direction
    [SerializeField] private float flipWaitTime;

    //Distance checks for ledge and wall detection
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;

    //Layer mask to identify ground surfaces
    [SerializeField] private LayerMask whatIsGround;

    //Initializes the Crawler's Rigidbody and sets gravity scale.
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f; //Set custom gravity for this enemy
    }

    //Detects collisions with other enemies and triggers flip behavior.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ChangeState(EnemyStates.Crawler_Flip);
        }
    }

    //Updates enemy states based on current behavior and environment.
    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f); //Trigger death if health is zero or below
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                HandleIdleState();
                break;

            case EnemyStates.Crawler_Flip:
                HandleFliplayerStatus();
                break;
        }
    }

    //Handles the Idle state behavior, including patrolling and ledge/wall detection.
    private void HandleIdleState()
    {
        //Position for ledge detection, based on facing direction
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        //Check for ledges or walls and trigger a state change
        if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
            || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
        {
            ChangeState(EnemyStates.Crawler_Flip);
        }

        //Adjust velocity based on facing direction
        rb.velocity = new Vector2(transform.localScale.x > 0 ? speed : -speed, rb.velocity.y);
    }

    //Handles the Flip state, causing the Crawler to wait and then change direction.
    private void HandleFliplayerStatus()
    {
        timer += Time.deltaTime;

        if (timer > flipWaitTime)
        {
            timer = 0;
            //Flip the Crawler's direction
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            ChangeState(EnemyStates.Crawler_Idle); //Return to idle after flipping
        }
    }
}