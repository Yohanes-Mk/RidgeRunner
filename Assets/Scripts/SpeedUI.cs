using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeedUI : MonoBehaviour
{
    public RCC_CarControllerV3 car;   // assign your car here
    public Text speedText;

    private float maxSpeed = 0f;
    public Text maxSpeedText;   // Assign in inspector


    void Update()
    {
        if (car == null) return;

        float speed = car.speed;

        // Update current speed
        speedText.text = speed.ToString("0") + " km/h";

        // Track max speed
        if (speed > maxSpeed)
            maxSpeed = speed;

        // Display max speed UI
        if (maxSpeedText != null)
            maxSpeedText.text = "Max: " + maxSpeed.ToString("0") + " km/h";
    }

    // Optional helper to set the car from another script
    public void SetCar(RCC_CarControllerV3 spawnedCar)
    {
        car = spawnedCar;
    }
}
