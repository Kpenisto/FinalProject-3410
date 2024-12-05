/*  
-------------------------------------------  
RespawnPoint.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/03/2024  
  
Purpose:  
This script handles the activation of respawn points when the player enters the trigger zone.  
It updates the respawn location for the player to the position of the respawn point.  
  
Key Features:  
Detects when the player enters the respawn point trigger zone.  
Updates the game manager’s respawn point to the position of the activated respawn point.  
Simple trigger-based system for saving respawn location.  
  
Changelog:  
-------------------------------------------  
11/03/2024 - Initial version created. Implemented OnTriggerEnter2D method to detect when the player enters the respawn point.  
             Updated `platformingRespawnPoint` in `GameManager` to store the position of the activated respawn point.  
             Added debug log message to confirm when the respawn point is activated.  
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    //Trigger method for when the player enters the respawn point
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            //Debug.Log("Respawn Point activated");
            //Updates the respawn point in GameManager to the current position of the respawn point
            GameManager.Instance.platformingRespawnPoint = transform.position;
        }
    }
}