/*  
-------------------------------------------  
SceneTransition.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/23/2024  
  
Purpose:  
Handles scene transitions by controlling the player's entry and exit points, and applying fade effects for smooth transitions between scenes.  
  
Key Features:  
Manages the player's spawn position when transitioning from a scene.
Triggers the fade-out effect when the scene starts.
Handles player collision to transition to the next scene with a fade-in effect.
Stores the name of the scene the player is transitioning from.

Changelog:  
-------------------------------------------  
11/24/2024 - Initial version created. Added the functionality to transition from a specific scene to another, based on trigger collision.
             Implemented the player's starting position based on the transition source.
             Applied fade-in and fade-out effects during the scene transition.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string transistionTo;  //The name of the scene to transition to
    [SerializeField] private Transform startPoint;  //The point where the player will spawn in the new scene
    [SerializeField] private Vector2 exitDirection;  //The direction the player exits
    [SerializeField] private float exitTime;  //Duration of the exit effect

    //Start is called before the first frame update
    private void Start()
    {
        //Check if the player is returning to the scene they came from; if so, set their spawn point
        if (transistionTo == GameManager.Instance.transitionedFromScene)
        {
            PlayerController.Instance.transform.position = startPoint.position;
        }

        //Start the fade-out effect when the scene starts
        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
    }

    //Triggered when the player enters the transition area
    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            //Store the current scene name for reference in the next scene
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            //Set player to cutscene mode and invincible state for the transition
            PlayerController.Instance.playerStatus.cutscene = true;
            PlayerController.Instance.playerStatus.invincible = true;

            //Load the new scene
            SceneManager.LoadScene(transistionTo);

            //Start the fade-in effect after loading the new scene
            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, transistionTo));
        }
    }
}