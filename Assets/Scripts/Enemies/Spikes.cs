/*  
-------------------------------------------  
Spikes.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/16/2024  
  
Purpose:  
Handles the respawn process when the player collides with a spike enemy. When triggered, the player will respawn at the designated platforming respawn point.  
  
Key Features:  
Detects when the player collides with the spikes.  
Pauses the game and triggers a fade-in effect.  
Takes damage and resets the player's position to the last platforming respawn point.  
Restores the game state after the respawn with a fade-out effect.  
  
Changelog:  
-------------------------------------------  
11/16/2024 - Initial version created. Implemented collision detection with the player.  
             Paused the game, triggered a fade-in effect, and initiated the respawn process.  
             Resets the player's position to the last saved respawn point after a brief delay.  
             Restores the game state by resetting invincibility and cutscene flags and resuming time.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    //Trigger method to handle player collision with spikes
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            StartCoroutine(RespawnPoint());
        }
    }

    //Coroutine to handle respawn process
    IEnumerator RespawnPoint()
    {
        //Enable cutscene mode and make player invincible during respawn
        PlayerController.Instance.playerStatus.cutscene = true;
        PlayerController.Instance.playerStatus.invincible = true;
        PlayerController.Instance.rb.velocity = Vector2.zero;

        //Pause the game, trigger fade-in, and initiate damage
        Time.timeScale = 0;
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        PlayerController.Instance.TakeDamage(1);

        //Wait for respawn delay and reset player position
        yield return new WaitForSecondsRealtime(1f);
        PlayerController.Instance.transform.position = GameManager.Instance.platformingRespawnPoint;

        //Trigger fade-out and restore game state
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        //Reset cutscene and invincibility states
        PlayerController.Instance.playerStatus.cutscene = false;
        PlayerController.Instance.playerStatus.invincible = false;

        //Resume normal game time
        Time.timeScale = 1;
    }
}