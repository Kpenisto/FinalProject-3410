/*  
-------------------------------------------  
HealthController.cs  
-------------------------------------------  
  
Author: Kyle Pensiton  
Date Created: 10/12/2024  
  
Purpose:  
This script manages the player's health display using heart containers. It dynamically updates the HUD based on the player's current and maximum health.  
  
Key Features:  
Initializes heart containers and fills based on the player's maximum health.  
Dynamically updates the HUD when the player's health changes.  
Manages heart container visibility and fill levels.  
  
Changelog:  
-------------------------------------------  
10/12/2024 - Initial version created. Added heart container initialization and dynamic health display.
12/01/2024 - Code cleanup and organization. Refactored variable names for clarity and consistency
             Grouped related fields together for better readability (serialized fields). Removed redundant or unused code.
-------------------------------------------  
*/

using UnityEngine;
using UnityEngine.UI;

public class HealthController : MonoBehaviour
{
    PlayerController player; //Reference to the PlayerController for health and event callbacks.  

    private GameObject[] heartContainers; //Array to store heart container GameObjects.  
    private Image[] heartFills; //Array to store Image components for heart fills.  

    public Transform heartsParent; //Parent transform for heart container instances.  
    public GameObject heartContainerPrefab; //Prefab for creating heart containers.  

    void Start()
    {
        player = PlayerController.Instance;

        //Initialize heart containers and fills based on the player's max health.  
        heartContainers = new GameObject[PlayerController.Instance.maxHealth];
        heartFills = new Image[PlayerController.Instance.maxHealth];

        //Subscribe to health change events.  
        PlayerController.Instance.onHealthChangedCallback += UpdateHeartsHUD;

        InstantiateHeartContainers(); //Instantiate heart containers at game start.  
        UpdateHeartsHUD(); //Initial HUD update.  
    }

    //Sets active heart containers based on max health.  
    void SetHeartContainers()
    {
        for (int i = 0; i < heartContainers.Length; i++)
        {
            heartContainers[i].SetActive(i < PlayerController.Instance.maxHealth);
        }
    }

    //Updates the fill level of each heart based on the player's current health.  
    void SetFilledHeart()
    {
        for (int i = 0; i < heartFills.Length; i++)
        {
            heartFills[i].fillAmount = i < PlayerController.Instance.Health ? 1 : 0;
        }
    }

    //Instantiates heart containers and assigns their fill Image components.  
    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);

            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    //Updates the heart container visibility and fill levels.  
    void UpdateHeartsHUD()
    {
        SetHeartContainers();
        SetFilledHeart();
    }
}