/*  
-------------------------------------------  
CameraFollow.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/05/2024  
  
Purpose:  
This script manages the camera's position to smoothly follow the player during gameplay. The camera's movement is interpolated for a smooth transition and includes an offset for optimal framing of the player.  
  
Key Features:  
Smooth camera follow behavior using linear interpolation (Lerp).  
Singleton pattern ensures only one active camera follow instance.  
Persistent through scene loads using `DontDestroyOnLoad`.  
  
Changelog:  
-------------------------------------------  
10/05/2024 - Initial version created. Implemented smooth camera follow with offset and singleton pattern. 
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance; //Singleton instance of CameraFollow.  
    [SerializeField] private float followSpeed = 0.1f; //Speed at which the camera follows the player.  
    [SerializeField] private Vector3 offset; //Offset to position the camera relative to the player.  

    private void Awake()
    {
        //Ensure only one instance of CameraFollow exists.  
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject); //Keep the camera persistent across scenes.  
    }

    void Update()
    {
        //Smoothly move the camera to follow the player with the specified offset.  
        transform.position = Vector3.Lerp(
            transform.position,
            PlayerController.Instance.transform.position + offset,
            followSpeed
        );
    }
}