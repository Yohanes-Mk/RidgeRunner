using UnityEngine;

public class CarSelector : MonoBehaviour
{
    [Header("RCC HUD")]
    public GameObject rccCanvas; // assign your RCC Canvas here

    [Header("Car Selection")]
    public GameObject[] carPrefabs;        // list of car prefabs
    public Transform spawnPoint;           // where car spawns
    public GameObject carSelectionPanel;   // assign your UI panel here
    [Header("Lap Manager")]
    public LapManager lapManager;
    [Header("Camera &")]
    public RCC_Camera mainCamera;          // your main RCC camera

    [Header("MiniMap")]
    public MiniMap miniMap;             // **<-- NEW: Drag the MiniMap script/GameObject here**
    public Transform miniMapPlayerIcon;  // **<-- NEW: Drag the PlayerIcon Sprite/Image Transform here**
    public SpeedUI speedUI;
    [Header("Optional HUD")]
    public GameObject gameHUD;             // assign your HUD canvas or panel
  
    private GameObject currentCar;
  
    void Start()
    {
        if (rccCanvas != null)
            rccCanvas.SetActive(false); // hide at start
        if (carSelectionPanel != null)
            carSelectionPanel.SetActive(true);
    }

    // Called by UI buttons
    public void SelectCar(int index)
    {
        // Destroy previous car if exists
        if (currentCar != null)
            Destroy(currentCar);

        // Spawn selected car prefab
        currentCar = Instantiate(carPrefabs[index], spawnPoint.position, spawnPoint.rotation);

        // Enable RCC Canvas now that car spawned
        if (rccCanvas != null)
            rccCanvas.SetActive(true);
      
        if (lapManager != null)
            lapManager.playerCar = currentCar.transform;
        // Assign car to RCC_Camera
        if (mainCamera != null)
            mainCamera.SetPlayerCar(currentCar);

        //  NEW: ASSIGN PLAYER TARGET TO MINIMAP
        if (miniMap != null && miniMapPlayerIcon != null)
        {
            // Pass the spawned car's Transform and the icon Transform to the MiniMap script.
            miniMap.SetPlayerTarget(currentCar.transform, miniMapPlayerIcon);
        }

        // Assign car to Speed UI
        var rccController = currentCar.GetComponent<RCC_CarControllerV3>();
        if (rccController != null)
        {
            rccController.enabled = true;

            // Assign to SpeedUI using the same variable
            if (speedUI != null)
                speedUI.SetCar(rccController);
        }


        // Enable RCC controller if disabled
        if (rccController != null)
            rccController.enabled = true;

        // Hide car selection panel
        if (carSelectionPanel != null)
            carSelectionPanel.SetActive(false);

        // Show game HUD
        if (gameHUD != null)
            gameHUD.SetActive(true);
    }
}
