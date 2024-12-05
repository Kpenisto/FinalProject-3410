/* 
-------------------------------------------
PlayerController.cs
-------------------------------------------

Author: Kyle Pensiton
Date Created: 10/5/2024

Purpose:
This script manages the player's core mechanics, providing a comprehensive system for character control and interaction within the game world. It includes functionality for movement, jumping, dashing, attacking, health and mana management, and more.

Key Features:
Movement: Handles both horizontal and vertical movement, including variable jump heights and air control.
Jumping: Implements ground detection, coyote time, and jump buffering for smooth and responsive jumping. Supports multiple air jumps with animation syncing.
Dashing: Adds a directional dash mechanic with cooldown management and animation support.
Attacking: Integrates a recoil system for impact feedback and allows directional attacks based on player input.
Health Management: Tracks player health, with clamped values and callbacks for UI updates. Implements invincibility frames with a flashing effect during healing or post-damage states.
Healing: Allows the player to heal under specific conditions, draining mana while incrementing health over time.
Hit-Stop Effect: Temporarily slows game time on impacts or specific events, enhancing feedback and visual intensity, with gradual restoration of normal time scale.
Death and Respawn: Handles player death, including death animations, visual effects, and a smooth respawn system.


Changelog:
-------------------------------------------
10/05/2024 - Initial version created. Added horizontal and vertical movement logic.
10/06/2024 - Implemented ground check and jumping mechanics.
10/11/2024 - Integrated attack system with directional checks. Added dashing functionality with cooldown. Included recoil system for attacks and impacts.
             Added hit-stop effect with time scaling and restoration logic. Improved time scale management and delayed restoration after a hit-stop event.
10/12/2024 - Implemented health system and invincibility frames. Added mana management and spellcasting logic.
             Finalized invincibility flashing logic during healing and invincibility state. Enhanced health/mana stat management with UI integration. 
10/21/2024 - Fixed bug with air jump logic and animation triggers. Refactored dash routine and jump variables logic.
10/22/2024 - Refined healing mechanics, including mana drain and heal timer logic. Updated ground check and air jump behavior. Added custom jump buffer and space walk time handling.
10/25/2024 - Finalized invincibility flashing logic during healing and invincibility state. Enhanced health/mana stat management with UI integration. 
             Fixed bug with air jump logic and animation triggers.
11/3/2024 -  Integrated respawn functionality and UI
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------
*/

using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //Horizontal Movement Settings
    [Header("Horizontal Movement Settings:")]
    [SerializeField] private float walkSpeed = 1f; //Controls the player's walking speed
    [Space(5)]

    //Vertical Movement Settings
    [Header("Vertical Movement Settings")]
    [SerializeField] private float jumpForce = 45f; //Controls the jump height
    private float jumpBufferCounter = 0f; //Stores jump input for buffering
    [SerializeField] private float jumpBufferFrames; //Max frames for jump input buffering
    private float spaceWalkCounter = 0f; //Stores how long the player can still jump after falling
    [SerializeField] private float spaceWalk; //Max duration for coyote time
    private int airJumpCounter = 0; //Tracks how many air jumps the player has performed
    [SerializeField] private int maxAirJumps; //Maximum number of air jumps allowed
    [SerializeField] private int maxFallingSpeed; //Limits the maximum falling speed
    private float gravity; //Stores the original gravity scale
    [Space(5)]

    //Ground Check Settings
    [Header("Ground Check Settings:")]
    [SerializeField] private Transform groundCheckPoint; //Position for checking if grounded
    [SerializeField] private float groundCheckY = 0.2f; //Vertical ground check range
    [SerializeField] private float groundCheckX = 0.5f; //Horizontal ground check range
    [SerializeField] private LayerMask whatIsGround; //Layer mask for ground detection
    [Space(5)]

    //Dash Settings
    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed; //Speed of dashing
    [SerializeField] private float dashTime; //Duration of a single dash
    [SerializeField] private float dashCooldown; //Cooldown time between dashes
    [SerializeField] GameObject dashEffect; //Visual effect for dashing
    private bool canDash = true, dashed; //Flags for dash availability
    [Space(5)]

    //Attack Settings
    [Header("Attack Settings:")]
    [SerializeField] private Transform SideAttackTransform; //Center point for side attacks
    [SerializeField] private Vector2 SideAttackArea; //Size of the side attack hitbox
    [SerializeField] private Transform UpAttackTransform; //Center point for upward attacks
    [SerializeField] private Vector2 UpAttackArea; //Size of the upward attack hitbox
    [SerializeField] private Transform DownAttackTransform; //Center point for downward attacks
    [SerializeField] private Vector2 DownAttackArea; //Size of the downward attack hitbox
    [SerializeField] private LayerMask attackableLayer; //Layer of attackable objects
    [SerializeField] private float timeBetweenAttack; //Minimum time between consecutive attacks
    private float timeSinceAttack; //Tracks time since last attack
    [SerializeField] private float damage; //Damage value for attacks
    [SerializeField] private GameObject slashEffect; //Visual effect for attacks
    public bool restoreTime;
    float restoreTimeSpeed;
    [Space(5)]

    //Recoil Settings
    [Header("Recoil Settings:")]
    [SerializeField] private int recoilXSteps = 5; //Number of horizontal recoil steps
    [SerializeField] private int recoilYSteps = 5; //Number of vertical recoil steps
    [SerializeField] private float recoilXSpeed = 100f; //Speed of horizontal recoil
    [SerializeField] private float recoilYSpeed = 100f; //Speed of vertical recoil
    private int stepsXRecoiled, stepsYRecoiled; //Tracks the number of recoil steps taken
    [Space(5)]

    //Health Settings
    [Header("Health Settings")]
    public int health; //Current health of the player
    public int maxHealth; //Maximum health
    [SerializeField] GameObject bloodSpurt; //Visual effect for taking damage
    [SerializeField] float hitFlashSpeed; //Flash speed when hit
    public delegate void OnHealthChangedDelegate(); //Delegate for health change events
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;
    private float healTimer; //Timer for passive healing
    [SerializeField] private float timeToHeal; //Time before passive healing starts
    [Space(5)]

    //Mana Settings
    [Header("Mana Settings")]
    [SerializeField] UnityEngine.UI.Image manaStorage; //UI image for mana display
    [SerializeField] private float mana; //Current mana
    [SerializeField] private float manaDrainSpeed; //Mana drain rate
    [SerializeField] private float manaGain; //Mana gain per successful action
    [Space(5)]

    //Spell Settings
    [Header("Spell Settings")]
    private float timeSinceCast; //Time since last spell cast
    private float HealTimer; //Timer for spell-based healing
    [Space(5)]

    [HideInInspector] public PlayerStateList playerStatus; //Reference to the player's state list
    private Animator anim; //Reference to Animator component
    public Rigidbody2D rb; //Reference to Rigidbody2D component
    private SpriteRenderer sr; //Reference to SpriteRenderer component
    private AudioSource audioSource; //Reference to AudioSource component

    //Input Variables
    private float xAxis, yAxis; //Tracks horizontal and vertical input
    private bool attack = false; //Tracks if attack input is pressed

    private bool canFlash = true; //Controls flashing during invincibility frames

    public static PlayerController Instance; //Singleton instance of PlayerController

    //Ensure only one PlayerController exists
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject); //Persist this object between scenes
    }

    //Initialize references and variables
    void Start()
    {
        playerStatus = GetComponent<PlayerStateList>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        gravity = rb.gravityScale; //Store default gravity
        manaStorage.fillAmount = mana; //Initialize mana bar
    }

    //Update is called once per frame
    void Update()
    {
        if (playerStatus.alive)
        {
            GetInputs(); //Gather player input
        }

        UpdateJumpVariables(); //Update jump-related variables
        RestoreTimeScale(); //Gradually restore normal time scale

        if (playerStatus.dashing) return;
        FlashWhileInvincible(); //Handle flashing effect during invincibility
        Heal(); //Handle healing logic

        if (playerStatus.healing) return;
        Move(); //Handle player movement
        Flip(); //Flip player sprite based on direction
        Jump(); //Handle jumping
        StartDash(); //Handle dashing logic
        Attack(); //Handle attacking
    }

    //FixedUpdate is called at a fixed interval
    private void FixedUpdate()
    {
        if (playerStatus.cutscene) return;

        if (playerStatus.dashing || playerStatus.healing) return;

        Recoil(); //Handle recoil mechanics
    }

    //Handles user input for movement, attack, and healing.
    void GetInputs()
    {
        //Get horizontal and vertical input axes (WASD or arrow keys)
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");

        //Detect attack input (button press)
        attack = Input.GetButtonDown("Attack");

        //Detect if the player is holding the Cast/Heal button
        if (Input.GetButton("Cast/Heal"))
        {
            HealTimer += Time.deltaTime; //Increase the healing timer if the button is pressed
        }
    }

    //Flips the character sprite based on the direction of movement.
    void Flip()
    {
        //Flip the character's horizontal scale to face the appropriate direction
        if (xAxis < 0)
        {
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            playerStatus.lookingRight = false;
        }
        else if (xAxis > 0)
        {
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            playerStatus.lookingRight = true;
        }
    }

    //Handles character movement by modifying the Rigidbody2D's velocity.
    private void Move()
    {
        //If healing, stop all movement
        if (playerStatus.healing) rb.velocity = new Vector2(0, 0);

        //Move the character horizontally based on input, preserving vertical velocity (jumping/falling)
        rb.velocity = new Vector2(walkSpeed * xAxis, rb.velocity.y);

        //Trigger walking animation if moving
        anim.SetBool("Walking", rb.velocity.x != 0 && Grounded());
    }

    //Starts the dash if conditions are met and initiates the Dash coroutine.
    void StartDash()
    {
        //If dash button is pressed, and dash conditions are met
        if (Input.GetButtonDown("Dash") && canDash && !dashed)
        {
            StartCoroutine(Dash()); //Start the Dash coroutine
            dashed = true; //Mark as dashed to prevent multiple dashes in one input
        }

        //Reset dash status when grounded
        if (Grounded())
        {
            dashed = false;
        }
    }

    //Coroutine for performing the dash action.
    IEnumerator Dash()
    {
        //Disable further dashes during the dash
        canDash = false;

        playerStatus.dashing = true;
        anim.SetTrigger("Dashing"); //Trigger the dash animation

        rb.gravityScale = 0; //Disable gravity during the dash

        //Set the dash direction based on the character's facing direction
        int _dir = playerStatus.lookingRight ? 1 : -1;
        rb.velocity = new Vector2(_dir * dashSpeed, 0); //Apply dash speed to velocity

        //Instantiate dash effect if grounded
        if (Grounded()) Instantiate(dashEffect, transform);

        //Wait for the dash to finish, then restore gravity
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;

        playerStatus.dashing = false; //End dashing state
        yield return new WaitForSeconds(dashCooldown); //Dash cooldown
        canDash = true; //Re-enable dashes after cooldown
    }

    //Handles attack logic and triggers the appropriate attack animations.
    void Attack()
    {
        //Track the time between attacks
        timeSinceAttack += Time.deltaTime;

        //If attack input is pressed and time since last attack is enough
        if (attack && timeSinceAttack >= timeBetweenAttack)
        {
            timeSinceAttack = 0; //Reset attack timer
            anim.SetTrigger("Attacking"); //Trigger the attack animation

            //Play attack sound effect
            AudioSource[] sounds = GetComponents<AudioSource>();
            sounds[0].Play();

            //Perform different attacks based on the vertical axis input (direction)
            if (yAxis == 0 || yAxis < 0 && Grounded())
            {
                int _recoildLeftOrRight = playerStatus.lookingRight ? 1 : -1;
                //Perform a side attack with recoil effect
                Hit(SideAttackTransform, SideAttackArea, ref playerStatus.recoilingX, Vector2.right * _recoildLeftOrRight, recoilXSpeed);
                Instantiate(slashEffect, SideAttackTransform); //Instantiate the slash effect
            }
            else if (yAxis > 0)
            {
                //Perform an upward attack with recoil effect
                Hit(UpAttackTransform, UpAttackArea, ref playerStatus.recoilingY, Vector2.up, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, 80, UpAttackTransform); //Instantiate slash effect at angle
            }
            else if (yAxis < 0 && !Grounded())
            {
                //Perform a downward attack with recoil effect
                Hit(DownAttackTransform, DownAttackArea, ref playerStatus.recoilingY, Vector2.down, recoilYSpeed);
                SlashEffectAtAngle(slashEffect, -90, DownAttackTransform); //Instantiate slash effect at angle
            }
        }
    }

    //Handles collision detection during an attack and applies recoil.
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool, Vector2 _recoilDir, float _recoilStrength)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);

        if (objectsToHit.Length > 0)
        {
            _recoilBool = true; //Set recoil flag if objects are hit
        }

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                //Apply damage and recoil to enemies
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage, _recoilDir, _recoilStrength);

                if (objectsToHit[i].CompareTag("Enemy"))
                {
                    //Gain mana if an enemy is hit
                    Mana += manaGain;
                }
            }
        }
    }

    //Instantiates the slash effect and applies rotation and scale.
    void SlashEffectAtAngle(GameObject _slashEffect, int _effectAngle, Transform _attackTransform)
    {
        _slashEffect = Instantiate(_slashEffect, _attackTransform); //Instantiate slash effect at attack position
        _slashEffect.transform.eulerAngles = new Vector3(0, 0, _effectAngle); //Rotate the effect
        _slashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y); //Match character scale
    }

    //Handles recoil after attacking, applying movement and gravity changes.
    void Recoil()
    {
        //Recoil logic for horizontal movement (if recoiling in X direction)
        if (playerStatus.recoilingX)
        {
            if (playerStatus.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0); //Apply recoil in the opposite direction
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0); //Apply recoil in the opposite direction
            }
        }

        //Recoil logic for vertical movement (if recoiling in Y direction)
        if (playerStatus.recoilingY)
        {
            rb.gravityScale = 0; //Disable gravity during recoil
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed); //Apply recoil upwards
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed); //Apply recoil downwards
            }
            airJumpCounter = 0; //Reset air jump counter
        }
        else
        {
            rb.gravityScale = gravity; //Restore gravity if not recoiling in Y direction
        }

        //Stop recoil logic after recoil steps are completed
        if (playerStatus.recoilingX && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++; //Increment recoil steps
        }
        else
        {
            StopRecoilX(); //Stop horizontal recoil
        }
        if (playerStatus.recoilingY && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++; //Increment recoil steps
        }
        else
        {
            StopRecoilY(); //Stop vertical recoil
        }

        //Stop vertical recoil when grounded
        if (Grounded())
        {
            StopRecoilY();
        }
    }

    //Stops horizontal recoil and resets the recoil step count.
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        playerStatus.recoilingX = false;
    }

    //Stops vertical recoil and resets the recoil step count.
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        playerStatus.recoilingY = false;
    }

    //Handles taking damage, reduces health, and checks for death.
    public void TakeDamage(float _damage)
    {
        if (playerStatus.alive)
        {
            Health -= Mathf.RoundToInt(_damage); //Reduce health by damage amount
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(Death()); //Start death sequence if health is 0
            }
            else
            {
                StartCoroutine(StopTakingDamage()); //Start invincibility after damage
            }
        }
    }

    //Stops the player from taking further damage for a brief period and plays damage animation.
    IEnumerator StopTakingDamage()
    {
        playerStatus.invincible = true; //Set invincible state
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, transform.rotation); //Instantiate blood spurt effect
        Destroy(_bloodSpurtParticles, 0.4f); //Destroy effect after 0.4 seconds
        anim.SetTrigger("TakeDamage"); //Trigger damage animation

        yield return new WaitForSeconds(1f); //Wait for invincibility period
        playerStatus.invincible = false; //End invincibility
    }

    //Restores the time scale to normal when 'restoreTime' is true, gradually increasing it.
    void RestoreTimeScale()
    {
        //If restoration is in progress
        if (restoreTime)
        {
            //Gradually increase time scale back to 1 if it's below 1
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * restoreTimeSpeed;
            }
            else
            {
                //Set time scale to 1 and stop restoring time
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }

    //Flashes the player's sprite while invincible and not in a cutscene.
    void FlashWhileInvincible()
    {
        //If player is invincible and not in a cutscene
        if (playerStatus.invincible && !playerStatus.cutscene)
        {
            //Start flashing effect if conditions are met
            if (Time.timeScale > 0.2 && canFlash) { StartCoroutine(Flash()); }
        }
        else
        {
            //Ensure the sprite is visible when not flashing
            sr.enabled = true;
        }
    }

    //Flash effect that alternates the sprite's visibility.
    IEnumerator Flash()
    {
        //Toggle sprite visibility
        sr.enabled = !sr.enabled;
        canFlash = false; //Disable further flashes temporarily
        yield return new WaitForSeconds(0.1f); //Wait for a short time before allowing another flash
        canFlash = true; //Re-enable flashing
    }

    //Method to implement a hit-stop effect by changing the time scale temporarily and restoring it after a delay.
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed; //Set the speed at which time will restore after the hit-stop

        //If a delay is provided (greater than 0), start a coroutine to restore time after the delay
        if (_delay > 0)
        {
            //Stop any previous coroutine and start a new one to restore time after the specified delay
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            //If no delay is specified, immediately set restoreTime to true to start restoring time
            restoreTime = true;
        }

        //Set the time scale to the new value for hit-stop effect
        Time.timeScale = _newTimeScale;
    }

    //Coroutine to restore time after a specified delay in real-time (ignoring time scale).
    IEnumerator StartTimeAgain(float _delay)
    {
        //Wait for the specified amount of real-time (ignoring the time scale)
        yield return new WaitForSecondsRealtime(_delay);

        //After the delay, set restoreTime to true to start restoring the time scale to normal
        restoreTime = true;
    }

    //Handles the player's death, including playing the death animation and activating the death screen.
    IEnumerator Death()
    {
        playerStatus.alive = false; //Mark player as dead
        Time.timeScale = 1f; //Ensure time scale is normal during death sequence
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity); //Spawn blood effect
        Destroy(_bloodSpurtParticles, 1.5f); //Destroy the blood effect after 1.5 seconds
        anim.SetTrigger("Death"); //Trigger death animation

        yield return new WaitForSeconds(0.9f); //Wait for death animation to finish
        StartCoroutine(UIManager.Instance.ActivateDeathScreen()); //Activate death screen UI
    }

    //Respawns the player with full health and idle animation if dead.
    public void Respawned()
    {
            playerStatus.alive = true;
            Health = 4; //Reset health
            anim.Play("Player_Idle"); //Play idle animation
    }

    //Property for managing the player's health with a callback on change.
    public int Health
    {
        get { return health; }
        set
        {
            //Only update health if the value changes
            if (health != value)
            {
                health = Mathf.Clamp(value, 0, maxHealth); //Clamp health between 0 and maxHealth

                //Invoke the callback if assigned
                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }

    //Heals the player if conditions are met (holding the heal button, not dashing, etc.)
    void Heal()
    {
        //Check if the player can heal (conditions like holding button, health < max, mana > 0, grounded, and not dashing)
        if (Input.GetButton("Cast/Heal") && HealTimer > 0.01f && Health < maxHealth && Mana > 0 && Grounded() && !playerStatus.dashing)
        {
            rb.velocity = new Vector2(0, 0);
            playerStatus.healing = true; //Set healing state to true
            anim.SetBool("Healing", true); //Trigger healing animation

            //Increase healing timer
            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++; //Heal by 1
                healTimer = 0; //Reset healing timer
            }

            //Drain mana over time while healing
            Mana -= Time.deltaTime * manaDrainSpeed;
        }
        else
        {
            //If healing is not allowed, reset healing state and animation
            playerStatus.healing = false;
            anim.SetBool("Healing", false);
            healTimer = 0;
        }
    }

    //Property for managing the player's mana with a UI update on change.
    float Mana
    {
        get { return mana; }
        set
        {
            //If mana changes, update it and the UI (mana bar)
            if (mana != value)
            {
                mana = Mathf.Clamp(value, 0, 1); //Clamp mana between 0 and 1
                manaStorage.fillAmount = Mana; //Update the mana bar
            }
        }
    }

    //Checks if the player is grounded using raycasts.
    public bool Grounded()
    {
        //Perform raycast checks in three positions around the player to detect if grounded
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true; //Player is grounded
        }
        else
        {
            return false; //Player is not grounded
        }
    }

    //Handles the player's jump logic, including jump buffering and air jumps.
    void Jump()
    {
        //Check if jump buffer is valid and coyote time is available
        if (jumpBufferCounter > 0 && spaceWalkCounter > 0 && !playerStatus.jumping)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce); //Apply jump force
            playerStatus.jumping = true; //Set jumping state to true
        }

        //If not grounded, allow air jumps
        if (!Grounded() && airJumpCounter < maxAirJumps && Input.GetButtonDown("Jump"))
        {
            playerStatus.jumping = true;
            airJumpCounter++; //Increment air jump counter
            rb.velocity = new Vector3(rb.velocity.x, jumpForce); //Apply jump force
        }

        //Adjust jump force based on input release (to prevent high jumps)
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 3)
        {
            playerStatus.jumping = false;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -maxFallingSpeed, rb.velocity.y)); //Limit falling speed
        }

        anim.SetBool("Jumping", !Grounded()); //Update jumping animation state
    }

    //Updates jump variables, including jump buffering and coyote time.
    void UpdateJumpVariables()
    {
        //If grounded, reset jumping and counters
        if (Grounded())
        {
            playerStatus.jumping = false;
            spaceWalkCounter = spaceWalk; //Reset coyote time
            airJumpCounter = 0; //Reset air jump counter
        }
        else
        {
            spaceWalkCounter -= Time.deltaTime; //Decrease coyote time while in the air
        }

        //Manage jump buffer (allow jumping shortly after pressing jump)
        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferFrames; //Buffer jump input
        }
        else
        {
            jumpBufferCounter = jumpBufferCounter - Time.deltaTime * 10; //Decrease jump buffer over time
        }
    }
}
