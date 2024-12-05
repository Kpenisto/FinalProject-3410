/*  
-------------------------------------------  
StartingRespawnPoint.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/03/2024  
  
Purpose:  
This script handles the activation of the starting respawn point when the player enters the trigger zone.  
It updates the respawn location for the player to the position of the starting respawn point.  
  
Key Features:  
Detects when the player enters the starting respawn point trigger zone.  
Updates the game manager’s starting respawn point to the position of the activated starting respawn point.  
Simple trigger-based system for saving starting respawn location.  
  
Changelog:  
-------------------------------------------  
11/03/2024 - Initial version created. Implemented OnTriggerEnter2D method to detect when the player enters the starting respawn point.  
             Updated `startingRespawnPoint` in `GameManager` to store the position of the activated starting respawn point.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class StartingRespawnPoint : MonoBehaviour
{
    //Trigger method for when the player enters the starting respawn point
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            //Debug.Log("Starting Respawn Point activated");
            //Updates the starting respawn point in GameManager to the current position of the starting respawn point
            GameManager.Instance.startingRespawnPoint = transform.position;
        }
    }
}