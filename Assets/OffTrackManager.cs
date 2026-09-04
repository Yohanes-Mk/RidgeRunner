using UnityEngine;
using UnityEngine.UI;

public class OffTrackTimer : MonoBehaviour
{
    public static OffTrackTimer Instance { get; private set; }
    [Header("Off-Track Settings")]
    [Tooltip("The RCC_CarControllerV3 script on the player vehicle.")]
    public RCC_CarControllerV3 carController;

    // CORRECTION: Multiplier for a 30% reduction (1.0 - 0.3 = 0.7)
    private const float PenaltyMultiplier = 0.7f;

    public GameObject offTrackUI;
    public Text liveOffTrackText;

    // REMOVED: public Text totalOffTrackText;

    private bool isOffTrack = false;
    private float currentOffTrackTime = 0f;
    private float totalOffTrackTime = 0f; // This variable is now public-facing via GetTotalOffTrackTime()

    private float originalEngineTorque = 0f;
    private float originalMaxSpeed = 0f;

    // REMOVED: Awake() method that referenced totalOffTrackText

    void Start()
    {

        Instance = this;
        // --- 1. Find Car Controller ---
        if (carController == null)
        {
            carController = GetComponentInParent<RCC_CarControllerV3>();
        }

        if (carController == null)
        {
            Debug.LogError("RCC_CarControllerV3 not found! Please assign it to the 'carController' field or place this script correctly.");
            return;
        }

        // --- 2. Store Original Physics Values ---
        originalEngineTorque = carController.engineTorque;
        originalMaxSpeed = carController.maxspeed;

        // --- 3. UI Setup ---
        offTrackUI = GameObject.FindGameObjectWithTag("OffTrackUI");
        if (offTrackUI != null)
        {
            liveOffTrackText = offTrackUI.GetComponentInChildren<Text>();
            offTrackUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("OffTrackUI not found! Please assign the tag correctly.");
        }

        // Removed totalOffTrackText setup
    }

    void Update()
    {
        if (isOffTrack)
        {
            // Update both timers
            currentOffTrackTime += Time.deltaTime;
            totalOffTrackTime += Time.deltaTime;


            // Update live off-track text in UI
            if (liveOffTrackText != null)
                liveOffTrackText.text = currentOffTrackTime.ToString("F2") + "s";
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OffTrack"))
        {
            isOffTrack = true;
            currentOffTrackTime = 0f;

            // APPLY 30% REDUCTION
            ApplyPenalty();

            if (offTrackUI != null)
                offTrackUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OffTrack"))
        {
            isOffTrack = false;

            // REMOVE PENALTY
            RemovePenalty();

            if (offTrackUI != null)
                offTrackUI.SetActive(false);

            if (liveOffTrackText != null)
                liveOffTrackText.text = "0.00s";

            currentOffTrackTime = 0f;
        }
    }

    // --- PENALTY LOGIC ---

    private void ApplyPenalty()
    {
        if (carController != null)
        {
            // Reduce Engine Torque by 30%
            carController.engineTorque = originalEngineTorque * PenaltyMultiplier;

            // Reduce Max Speed by 30%
            carController.maxspeed = originalMaxSpeed * PenaltyMultiplier;
        }
    }

    private void RemovePenalty()
    {
        if (carController != null)
        {
            // Restore original values
            carController.engineTorque = originalEngineTorque;
            carController.maxspeed = originalMaxSpeed;
        }
    }

    // --- Public Utility Methods ---

    // Removed: ShowTotalOffTrackTime() since it will be handled by LapManager

    public float GetTotalOffTrackTime()
    {
        return totalOffTrackTime;
    }
}