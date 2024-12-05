/*  
-------------------------------------------  
UIManager.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/23/2024  
  
Purpose:  
Manages the UI elements related to game events such as death and win conditions. Handles activation and deactivation of death and win screens, and controls scene transitions using `SceneFader`.  
  
Key Features:  
Controls the activation and deactivation of death and win screens with smooth transitions.  
Uses `SceneFader` to fade in and out during UI screen changes.  
Singleton pattern ensures only one instance of the UIManager exists throughout the game.  
  
Changelog:  
-------------------------------------------  
11/23/2024 - Initial version created. Implemented singleton pattern to ensure only one instance of `UIManager`.  
             Added methods to activate and deactivate the death and win screens with fade transitions.  
             Integrated `SceneFader` to handle fade-ins and fade-outs during screen transitions.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public SceneFader sceneFader;  //Reference to the SceneFader component
    public static UIManager Instance;  //Singleton instance for UIManager
    [SerializeField] GameObject deathScreen;  //Reference to the death screen UI element
    [SerializeField] GameObject winScreen;  //Reference to the win screen UI element

    //Singleton pattern to ensure a single instance of UIManager
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
        DontDestroyOnLoad(gameObject);  //Prevents UIManager from being destroyed when changing scenes
    }

    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();  //Get the SceneFader component from children
    }

    //Activates the death screen and fades it in
    public IEnumerator ActivateDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);  //Wait before showing death screen
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));  //Fade in effect

        yield return new WaitForSeconds(0.8f);  //Wait for fade to finish
        deathScreen.SetActive(true);  //Enable the death screen UI element
    }

    //Deactivates the death screen and fades it out
    public IEnumerator DeactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);  //Wait before hiding death screen
        deathScreen.SetActive(false);  //Disable the death screen UI element
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));  //Fade out effect
    }

    //Activates the win screen and fades it in
    public IEnumerator ActivateWinScreen()
    {
        PlayerController.Instance.playerStatus.invincible = true;
        yield return new WaitForSeconds(0.8f);  //Wait before showing win screen
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));  //Fade in effect
        winScreen.SetActive(true);  //Enable the win screen UI element
    }

    //Deactivates the win screen and fades it out
    public IEnumerator DeactivateWinScreen()
    {
        PlayerController.Instance.playerStatus.invincible = false;
        yield return new WaitForSeconds(0.2f);  //Wait before hiding win screen
        winScreen.SetActive(false);  //Disable the win screen UI element
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));  //Fade out effect
    }
}