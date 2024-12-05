/*  
-------------------------------------------  
FadeUI.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/23/2024  
  
Purpose:  
Handles fading in and out of UI elements using Unity's `CanvasGroup` to adjust the alpha (transparency) and interactivity.  
  
Key Features:  
Allows for smooth fading in and out of UI elements by manipulating their alpha values.
Prevents interaction with UI elements during fade out by disabling `interactable` and `blocksRaycasts` properties.
Uses `Time.unscaledDeltaTime` to ensure the fading is independent of game time scaling (paused states).  
  
Changelog:  
-------------------------------------------  
11/23/2024 - Initial version created. Added `FadeUIOut` and `FadeUIIn` methods for handling fade transitions.
             Integrated `CanvasGroup` to control transparency and interactivity of UI elements.
             Used coroutines to smoothly change the alpha value over time for fade effects. 
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;  //Reference to the CanvasGroup component

    private void Awake()
    {
        //Getting the CanvasGroup component attached to the same GameObject
        canvasGroup = GetComponent<CanvasGroup>();
    }

    //Initiates the fade-out effect over a specified duration
    public void FadeUIOut(float _seconds)
    {
        StartCoroutine(FadeOut(_seconds));
    }

    //Initiates the fade-in effect over a specified duration
    public void FadeUIIn(float _seconds)
    {
        StartCoroutine(FadeIn(_seconds));
    }

    //Coroutine to fade the UI element out (decrease alpha to 0)
    IEnumerator FadeOut(float _seconds)
    {
        canvasGroup.interactable = false;  //Disable interaction with the UI
        canvasGroup.blocksRaycasts = false;  //Prevent UI elements from blocking raycasts
        canvasGroup.alpha = 1;  //Start with full opacity
        while (canvasGroup.alpha > 0)
        {
            //Gradually decrease alpha value over time
            canvasGroup.alpha -= Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        yield return null;
    }

    //Coroutine to fade the UI element in (increase alpha to 1)
    IEnumerator FadeIn(float _seconds)
    {
        canvasGroup.alpha = 0;  //Start with fully transparent
        while (canvasGroup.alpha < 1)
        {
            //Gradually increase alpha value over time
            canvasGroup.alpha += Time.unscaledDeltaTime / _seconds;
            yield return null;
        }
        canvasGroup.interactable = true;  //Re-enable interaction with the UI
        canvasGroup.blocksRaycasts = true;  //Allow the UI elements to block raycasts
        yield return null;
    }
}