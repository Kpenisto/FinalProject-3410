/*  
-------------------------------------------  
MenuFadeController.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/24/2024  
  
Purpose:  
Controls the fading of UI elements in the main menu and handles scene transitions for starting the game and quitting.  
  
Key Features:  
Manages fading UI transitions when the game starts or when the player exits the game.
Calls the `FadeUI` class methods for smooth fade-in and fade-out effects.
Calls the `FadeUI` class methods for smooth fade-in and fade-out effects.
Loads a specified scene after the fade-in transition.
Provides a method for quitting the game.

Changelog:  
-------------------------------------------  
11/24/2024 - Initial version created. Added fade-out effect when the scene starts.
             Implemented `CallFadeAndStartGame` to handle the fade transition before loading the scene.
             Added `Quit` method to exit the game.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuFadeController : MonoBehaviour
{
    private FadeUI fadeUI;  //Reference to the FadeUI component
    [SerializeField] private float fadeTime;  //Duration for the fade effect

    //Start is called before the first frame update
    void Start()
    {
        fadeUI = GetComponent<FadeUI>();  //Get the FadeUI component attached to the same GameObject
        fadeUI.FadeUIOut(fadeTime);  //Apply the fade-out effect when the menu scene starts
    }

    //Method to trigger the fade-in effect and then load the specified scene
    public void CallFadeAndStartGame(string _sceneToLoad)
    {
        StartCoroutine(FadeAndStartGame(_sceneToLoad));  //Start the coroutine to fade and load the scene
    }

    //Method to quit the application (useful for a quit button in the main menu)
    public void Quit()
    {
        Application.Quit();  //Closes the application
    }

    //Coroutine that fades the UI in and then loads the specified scene
    IEnumerator FadeAndStartGame(string _sceneToLoad)
    {
        fadeUI.FadeUIIn(fadeTime);  //Apply the fade-in effect
        yield return new WaitForSeconds(fadeTime);  //Wait for the fade-in duration to finish
        SceneManager.LoadScene(_sceneToLoad);  //Load the desired scene
    }
}