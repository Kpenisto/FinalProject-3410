/*  
-------------------------------------------  
SceneFader.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 11/23/2024  
  
Purpose:  
Handles scene fading transitions for UI elements in the game. It controls fade-in and fade-out transitions when changing scenes or activating UI effects.  
  
Key Features:  
Provides functionality for fading in and out with adjustable timing.  
Supports scene loading with fade-out before transitioning.  
Can be reused for various UI effects like cutscene transitions, game over screens, etc.  
  
Changelog:  
-------------------------------------------  
11/23/2024 - Initial version created. Implemented fade-in and fade-out transitions using Unity's Image component.  
             Created `Fade` method to handle both fade directions.  
             Added `FadeAndLoadScene` method to fade out, then load a new scene.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    [SerializeField] public float fadeTime;  //Time duration for the fade effect
    private Image fadeOutUIImage;  //Image component to use for fading effect

    //Enum for fade direction (In or Out)
    public enum FadeDirection
    {
        In,  //Fade in (transition from transparent to opaque)
        Out  //Fade out (transition from opaque to transparent)
    }

    //Start is called before the first frame update
    private void Awake()
    {
        fadeOutUIImage = GetComponent<Image>();  //Get the Image component from the GameObject
    }

    //Coroutine to handle fading effects based on direction (in or out)
    public IEnumerator Fade(FadeDirection _fadeDirection)
    {
        float _alpha = _fadeDirection == FadeDirection.Out ? 1 : 0;  //Set initial alpha value based on fade direction
        float _fadeEndValue = _fadeDirection == FadeDirection.Out ? 0 : 1;  //Set end value of alpha based on fade direction

        if (_fadeDirection == FadeDirection.Out)  //Fade out: make the image transparent over time
        {
            while (_alpha >= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);  //Adjust image color and alpha value

                yield return null;  //Wait until the next frame
            }

            fadeOutUIImage.enabled = false;  //Disable image after fade-out is complete
        }
        else  //Fade in: make the image opaque over time
        {
            fadeOutUIImage.enabled = true;  //Ensure image is visible during fade-in

            while (_alpha <= _fadeEndValue)
            {
                SetColorImage(ref _alpha, _fadeDirection);  //Adjust image color and alpha value

                yield return null;  //Wait until the next frame
            }
        }
    }

    //Coroutine to fade out, then load a new scene
    public IEnumerator FadeAndLoadScene(FadeDirection _fadeDirection, string _sceneToLoad)
    {
        fadeOutUIImage.enabled = true;  //Make sure the fade image is visible

        yield return Fade(_fadeDirection);  //Perform fade transition

        SceneManager.LoadScene(_sceneToLoad);  //Load the specified scene after fade-out is complete
    }

    //Helper method to adjust the image color and alpha value over time
    void SetColorImage(ref float _alpha, FadeDirection _fadeDirection)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, _alpha);  //Set new alpha value on image

        //Update the alpha value based on fade direction and speed
        _alpha += Time.deltaTime * (1 / fadeTime) * (_fadeDirection == FadeDirection.Out ? -1 : 1);
    }
}