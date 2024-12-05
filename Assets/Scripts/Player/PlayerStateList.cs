/*
-------------------------------------------
PlayerStateList
-------------------------------------------
Author: Kyle Peniston
Date Created: 10/5/2024

Purpose:
This script manages and tracks the various states of the player character throughout gameplay.
It provides a centralized list of boolean flags representing different player states, such as 
movement, combat, and invulnerability, to be referenced by other systems.

Key Features:
Tracks player movement states (jumping, dashing).
Manages combat states (recoiling, invincibility, casting).
Handles player interaction states (healing, cutscenes).
Provides a flag for the player's life state (alive).
 
Changelog:
-------------------------------------------
10/06/2024 - Initial version created. Added core player state tracking, including jumping, dashing, and facing direction logic.
10/11/2024 - Combat state enhancements. Added recoil flags (recoilingX, recoilingY) to track directional knockback from impacts and attacks.
10/12/2024 - Health and invincibility integration. Added invincible state to track temporary immunity after taking damage. Added healing and casting states for spellcasting and healing actions.
11/02/2024 - Dash and movement refinement. Updated dashing state to align with improved dash mechanics.
             Finalized player death and respawn logic. Added alive state to manage player death and respawn sequences.
11/02/2024 - Cutscene state added. Introduced cutscene flag to disable player input during narrative events.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------
*/

using UnityEngine;

public class PlayerStateList : MonoBehaviour
{
    //Indicates whether the player is currently jumping.
    public bool jumping = false;

    //Indicates whether the player is currently dashing.
    public bool dashing = false;

    //Indicates whether the player is recoiling horizontally due to an impact or attack.
    public bool recoilingX;

    //Indicates whether the player is recoiling vertically due to an impact or attack.
    public bool recoilingY;

    //Indicates the direction the player is facing. True if looking right, false if looking left.
    public bool lookingRight;

    //Indicates whether the player is currently invincible (after taking damage).
    public bool invincible;

    //Indicates whether the player is currently performing a healing action.
    public bool healing;

    //Indicates whether the player is currently alive. False if the player is dead.
    public bool alive;

    //Indicates whether the player is in a cutscene, disabling certain controls.
    public bool cutscene = false;
}
