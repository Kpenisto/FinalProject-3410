/*  
-------------------------------------------  
RamController.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/16/2024  
  
Purpose:  
This script defines the behavior of the "Charger" enemy, including idle, surprise, and charging states.  
The Charger detects the player, jumps to surprise them, and then charges at the player.  
  
Key Features:  
Idle behavior with ledge detection and player proximity check.  
Surprise state where the Charger jumps before charging.  
Charging mechanic with increased speed and duration.  
  
Changelog:  
-------------------------------------------  
10/16/2024 - Initial version created. Added basic states for idle, surprise, and charge.  
10/18/2024 - Implemented jump logic during surprise state. Improved state transitions between idle, surprise, and charge.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;

public class RamController : Enemy
{
    //Timer to track charge duration
    private float timer;

    //Ledge detection parameters
    [SerializeField] private float ledgeCheckX;
    [SerializeField] private float ledgeCheckY;

    //Charging behavior parameters
    [SerializeField] private float chargeSpeedMultiplier;
    [SerializeField] private float chargeDuration;
    [SerializeField] private float jumpForce;

    //Ground detection for ledge and collision checking
    [SerializeField] private LayerMask whatIsGround;

    //Initializes the Charger enemy, setting it to idle state and adjusting gravity scale.
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Charger_Idle); //Set to idle at the start
        rb.gravityScale = 3f; //Set gravity scale for proper movement physics
    }


    //Detects collisions with other enemies and handles scale flipping when colliding.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //Flip the direction of the Charger when colliding with another enemy
            transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        }
    }

    //Updates the current state of the Charger, handling idle, surprised, and charge behaviors.
    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f); //Trigger death if health reaches zero
        }

        //Define direction for ledge and wall checks
        Vector3 _ledgeCheckStart = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:
                //Check if ledge is below or wall is detected; flip direction if necessary
                if (!Physics2D.Raycast(transform.position + _ledgeCheckStart, Vector2.down, ledgeCheckY, whatIsGround)
                    || Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, whatIsGround))
                    {
                        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y); //Flip direction
                    }

                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStart, _wallCheckDir, ledgeCheckX * 10);

                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    ChangeState(EnemyStates.Charger_Surprised); //After jumping, transition to charge state
                }


                //Move Charger depending on direction
                if (transform.localScale.x > 0)
                {
                    rb.velocity = new Vector2(speed, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-speed, rb.velocity.y);
                }
                break;

            case EnemyStates.Charger_Surprised:
                rb.velocity = new Vector2(2, jumpForce); //Make the Charger jump

                ChangeState(EnemyStates.Charger_Charge);
                break;

            case EnemyStates.Charger_Charge:
                timer += Time.deltaTime;

                if (timer < chargeDuration)
                {
                    //Keep charging while not exceeding the charge duration
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, whatIsGround))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rb.velocity = new Vector2(speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                        else
                        {
                            rb.velocity = new Vector2(-speed * chargeSpeedMultiplier, rb.velocity.y);
                        }
                    }
                    else
                    {
                        rb.velocity = new Vector2(0, rb.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Charger_Idle); //Reset to idle state after charge duration ends
                }
                break;
        }
    }

    //Changes the animation speed based on the current state.
    protected override void ChangeCurrentAnimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Charger_Idle)
        {
            anim.speed = 1; //Normal animation speed in idle state
        }

        if (GetCurrentEnemyState == EnemyStates.Charger_Charge)
        {
            anim.speed = chargeSpeedMultiplier; //Increase animation speed during charge
        }
    }
}