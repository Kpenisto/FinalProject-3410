/*  
-------------------------------------------  
Flyer.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/16/2024  
  
Purpose:  
This script defines the behavior of the "Flyer" enemy, including patrol, chase, stun, and death states.  
The Bat chases the player when within range and enters a stunned state upon taking damage.  
  
Key Features:  
Chase behavior when player is within a certain distance.  
Stun mechanic triggered by player damage.  
Death state with animation trigger and layer change for death effects.  
  
Changelog:  
-------------------------------------------  
10/16/2024 - Initial version created. Added basic states for idle, chase, stun, and death.  
10/18/2024 - Added flipping behavior to Bat to face the player. Integrated stun duration and transition to idle after stun.  
11/09/2024 - Implemented death mechanic with random destroy time and special handling for BossBat.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class Flyer : Enemy
{
    //Distance at which the bat will start chasing the player
    [SerializeField] private float chaseDistance;

    //Duration of the stun effect after being hit
    [SerializeField] protected float stunDuration;

    //Timer for managing the stun duration
    private float timer;

    //Initializes the bat's state and sets it to idle.
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle); //Set initial state to idle
    }

    //Updates the bat's state behavior, including chasing and stun logic.
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                rb.velocity = new Vector2(0, 0); //Stop movement in idle state
                if (_dist < chaseDistance) //If player is within chase range
                {
                    ChangeState(EnemyStates.Bat_Chase); //Start chasing the player
                }
                break;

            case EnemyStates.Bat_Chase:
                //Move towards the player while chasing
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                FlipBat(); //Flip the bat to face the player

                if (_dist > chaseDistance) //If the player moves out of range, return to idle
                {
                    ChangeState(EnemyStates.Bat_Idle);
                }
                break;

            case EnemyStates.Bat_Stunned:
                timer += Time.deltaTime;

                if (timer > stunDuration) //Transition back to idle after stun duration
                {
                    ChangeState(EnemyStates.Bat_Idle);
                    timer = 0;
                }
                break;

            case EnemyStates.Bat_Death:
                Death(Random.Range(1, 5)); //Trigger death with a random delay
                break;
        }
    }

    //Handles the bat taking damage, transitioning to stunned or death state.
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Bat_Stunned); //If health is still above 0, switch to stunned
        }
        else
        {
            ChangeState(EnemyStates.Bat_Death); //If health is 0 or below, trigger death state
        }
    }

    //Handles the death of the bat, including gravity adjustments and destruction.
    protected override void Death(float _destroyTime)
    {
        rb.gravityScale = 12; //Adjust gravity scale to make the bat fall realistically
        base.Death(_destroyTime); //Trigger base class death behavior
    }

    //Updates the bat's animations based on the current state.
    protected override void ChangeCurrentAnimation()
    {
        anim.SetBool("Idle", GetCurrentEnemyState == EnemyStates.Bat_Idle);
        anim.SetBool("Chase", GetCurrentEnemyState == EnemyStates.Bat_Chase);
        anim.SetBool("Stunned", GetCurrentEnemyState == EnemyStates.Bat_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Bat_Death)
        {
            anim.SetTrigger("Death"); //Trigger death animation
            int LayerIgnorePlayer = LayerMask.NameToLayer("Ignore Player");
            gameObject.layer = LayerIgnorePlayer; //Change the layer to "Ignore Player" for death

            //Special handling for the BossBat
            if (gameObject.name == "BossBee")
            {
                new WaitForSeconds(2f); //Wait before activating the win screen
                StartCoroutine(UIManager.Instance.ActivateWinScreen());
            }
        }
    }

    //Flips the bat to face the player depending on their position.
    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
}