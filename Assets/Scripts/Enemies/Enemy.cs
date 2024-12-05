/*  
-------------------------------------------  
Enemy.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/11/2024  
  
Purpose:  
This script serves as the base class for enemy behavior in the game. It manages core mechanics such as health, state transitions, recoil, and attacks.  
  
Key Features:  
Handles enemy health and damage mechanics, including recoil and death.  
Defines multiple states for various enemy types. 
Supports state-based animations and transitions.  
Manages enemy attack logic and interactions with the player.  
  
Changelog:  
-------------------------------------------  
10/11/2024 - Initial version created. Implemented basic enemy states and health system. Added recoil behavior and collision handling with player.  
10/12/2024 - Added blood effect instantiation on enemy hit.    
10/15/2024 - Implemented placeholder methods for animation state changes.  
             Enhanced enemy state management using a state enumeration.  
10/20/2024 - Added death logic with customizable destroy time delay.  
             Integrated animation trigger handling in state transitions.
11/10/2024 - Enhanced enemy AI by preparing distinct states for Crawler, Bat, Charger, and Boss.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health; //Enemy's health points.
    [SerializeField] protected float recoilLength; //Duration of recoil effect.
    [SerializeField] protected float recoilFactor; //Force multiplier for recoil.
    [SerializeField] protected bool isRecoiling = false; //Whether the enemy is currently recoiling.

    [SerializeField] public float speed; //Movement speed of the enemy.

    [SerializeField] protected float damage; //Damage dealt to the player.
    [SerializeField] protected GameObject orangeBlood; //Prefab for blood effect on hit.

    protected float recoilTimer; //Timer for recoil duration.
    protected Rigidbody2D rb; //Reference to Rigidbody2D for physics.
    protected SpriteRenderer sr; //Reference to SpriteRenderer for visuals.
    protected Animator anim; //Reference to Animator for handling animations.

    protected enum EnemyStates
    {
        //Crawler States
        Crawler_Idle,
        Crawler_Flip,

        //Bat States
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,

        //Charger States
        Charger_Idle,
        Charger_Surprised,
        Charger_Charge
    }
    protected EnemyStates currentEnemyState; //Current state of the enemy.

    //Property to get and set the current enemy state with animation change handling.
    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currentEnemyState; }
        set
        {
            if (currentEnemyState != value)
            {
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    //Start is called before the first frame update.
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    //Update is called once per frame.
    protected virtual void Update()
    {
        if (isRecoiling)
        {
            HandleRecoil();
        }
        else
        {
            UpdateEnemyStates();
        }
    }

    //Handles the enemy being hit, applying damage and recoil.
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            SpawnBloodEffect();
            ApplyRecoil(_hitDirection, _hitForce);
        }
    }

    //Handles collision with the player and triggers attack logic.
    protected virtual void OnCollisionStay2D(Collision2D _other)
    {
        if (_other.gameObject.CompareTag("Player") && !PlayerController.Instance.playerStatus.invincible && health > 0)
        {
            Attack();
            if (PlayerController.Instance.playerStatus.alive)
            {
                PlayerController.Instance.HitStopTime(0, 5, 0.5f);
            }
        }
    }

    //Destroys the enemy game object after a specified delay.
    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject, _destroyTime);
    }

    //Placeholder for updating enemy states.
    protected virtual void UpdateEnemyStates() { }

    //Placeholder for changing animations based on current state.
    protected virtual void ChangeCurrentAnimation() { }

    //Changes the current state of the enemy.
    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    //Handles enemy attack logic and damages the player.
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    //Manages the recoil effect timer.
    private void HandleRecoil()
    {
        if (recoilTimer < recoilLength)
        {
            recoilTimer += Time.deltaTime;
        }
        else
        {
            isRecoiling = false;
            recoilTimer = 0;
        }
    }

    //Instantiates blood effect on hit.
    private void SpawnBloodEffect()
    {
        GameObject _orangeBlood = Instantiate(orangeBlood, transform.position, Quaternion.identity);
        Destroy(_orangeBlood, 5.5f);
    }

    //Applies recoil force to the enemy.
    private void ApplyRecoil(Vector2 _hitDirection, float _hitForce)
    {
        rb.velocity = _hitForce * recoilFactor * _hitDirection;
        isRecoiling = true;
    }
}