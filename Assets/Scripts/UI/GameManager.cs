/*  
-------------------------------------------  
GameManager.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/02/2024  
  
Purpose:  
This script manages the overall game state, including respawn points, scene transitions, and game restart functionality.  
It handles player respawn, scene transitions, and quitting the game.  
  
Key Features:  
Manages respawn points for the player in various game scenes.  
Allows restarting and quitting the game.  
Prevents multiple instances of the GameManager using a singleton pattern.  
  
Changelog:  
-------------------------------------------  
11/02/2024 - Initial version created. Implemented singleton pattern to ensure only one instance of GameManager exists.  
             Added functionality for respawning the player at specific respawn points in platforming levels.  
             Implemented `RespawnPlayer` method to reset player position and handle respawn behavior.  
             Implemented `ExitGame` method for quitting the game (works in built version).  
             Created `RestartGame` method to reload the game scene and reset the player's position.  
             Ensured game data persistence across scene transitions using `DontDestroyOnLoad`.  
11/10/2024 - Added scene transition management. Integrated `transitionedFromScene` variable to track the previous scene for dynamic game flow.  
             Improved player respawn handling by adding death screen deactivation after respawn.  
             Refined scene reloading to ensure the player starts at the correct position.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Stores the scene from which the player transitioned
    public string transitionedFromScene;
    //The respawn point for platforming-related levels
    public Vector2 platformingRespawnPoint;
    //General respawn point for the player
    public Vector2 respawnPoint;
    //The starting respawn point for the player when the game begins
    public Vector2 startingRespawnPoint;

    //Singleton pattern to ensure only one GameManager exists
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        //Ensures there is only one instance of the GameManager in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        //Prevents the GameManager from being destroyed when transitioning between scenes
        DontDestroyOnLoad(gameObject);
    }

    //Respawns the player at the platforming respawn point and resets the necessary game states.
    public void RespawnPlayer()
    {
        respawnPoint = platformingRespawnPoint;
        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }

    //Quits the game when called (will only work in a built version, not in the editor).
    public void ExitGame()
    {
        Application.Quit();
    }

    //Restarts the game by reloading the initial scene and resetting the player's position.
    public void RestartGame()
    {
        StartCoroutine(UIManager.Instance.DeactivateWinScreen());
        SceneManager.LoadScene("Cave_1"); //Reloads the "Cave_1" scene
        respawnPoint = startingRespawnPoint;
        PlayerController.Instance.transform.position = respawnPoint;
        PlayerController.Instance.Respawned();
    }
}