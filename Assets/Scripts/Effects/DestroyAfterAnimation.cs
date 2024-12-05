/*  
-------------------------------------------  
DestroyAfterAnimation.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/12/2024  
  
Purpose:  
This script automatically destroys the GameObject after its animation finishes playing. It ensures that temporary objects, such as effects or visual cues, are removed from the scene once their purpose is complete.  
  
Key Features:  
Destroys the GameObject after the current animation completes.  
  
Changelog:  
-------------------------------------------  
10/12/2024 - Initial version created. Implemented destruction logic after animation completion.  
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class DestroyAfterAnimation : MonoBehaviour
{
    void Start()
    {
        //Automatically destroy this GameObject after the current animation completes.  
        Destroy(gameObject, GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
    }
}