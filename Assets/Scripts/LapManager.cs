using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class LapManager : MonoBehaviour
{
    [Header("Checkpoints")]
    public Transform[] checkpoints;

    [Header("Live UI")]
    public Text lap1Text;
    public Text lap2Text;
    public Text lap3Text;
    public Text bestLapText;
    public Text lapCounterText;
    public Text liveTimerText;
    public Text totalLapsText;

    [Header("End Stats Panel")]
    public GameObject statsPanel;
    public Text statsText;
    public GameObject gameUiPanel;
    public int totalLaps = 3;


    [HideInInspector] public Transform playerCar;
    public Camera cameraa;

    // --- NEW: Off-Track Timer Reference ---
    private float offTrackTimer;

    // --- TELEMETRY VARIABLES ---
    private float maxSpeedReached = 0f;
    private RCC_CarControllerV3 currentRccController;

    // Checkpoint Split Variables
    private float lastCheckpointTime = 0f;
    private List<string> checkpointSplits = new List<string>();
    // ---------------------------

    private int currentLap = 1;
    private int currentCheckpoint = 0;

    private float lapTimer = 0f;
    private float totalTime = 0f;
    private float[] lapTimes;

    private float sessionBestLap = 99999f;
    private const string BEST_OVERALL_KEY = "BestOverallTime";
    private const string BEST_LAP_EVER_KEY = "BestLapEver";
    private float bestOverallEver = 99999f;
    private float bestLapEver = 99999f;

    void Start()
    {
        // Find the OffTrackTimer on the player car
        //if (playerCar != null)
        //{
        //    offTrackTimer = playerCar.GetComponent<OffTrackTimer>();
        //}

        // Original Start logic
        lapTimes = new float[3];
        statsPanel.SetActive(false);

        bestOverallEver = PlayerPrefs.GetFloat(BEST_OVERALL_KEY, 99999f);
        bestLapEver = PlayerPrefs.GetFloat(BEST_LAP_EVER_KEY, 99999f);

        lap1Text.text = "Lap 1: --";
        lap2Text.text = "Lap 2: --";
        lap3Text.text = "Lap 3: --";
        bestLapText.text = bestLapEver < 99999f ? "Best Lap: " + bestLapEver.ToString("F2") + "s" : "Best Lap: --";
        lapCounterText.text = "Lap: 1/" + totalLaps;
        liveTimerText.text = "Time: 0.00";
    }

    void Update()
    {
        if (playerCar != null)
        {
            if (currentRccController == null)
                currentRccController = playerCar.GetComponent<RCC_CarControllerV3>();

            // Max Speed Telemetry
            if (currentRccController != null)
            {
                if (currentRccController.speed > maxSpeedReached)
                    maxSpeedReached = currentRccController.speed;
            }

            // Update timers
            lapTimer += Time.deltaTime;
            totalTime += Time.deltaTime;

            // LIVE RACE TIMER (NEVER RESETS)
            liveTimerText.text = "Time: " + totalTime.ToString("F2") + "s";

            // Lap counter
            lapCounterText.text = currentLap.ToString();
            totalLapsText.text = totalLaps.ToString();

            // LIVE LAP TIMER DISPLAY
            if (currentLap == 1)
                lap1Text.text = "Lap 1: " + lapTimer.ToString("F2") + "s";
            else if (currentLap == 2)
                lap2Text.text = "Lap 2: " + lapTimer.ToString("F2") + "s";
            else if (currentLap == 3)
                lap3Text.text = "Lap 3: " + lapTimer.ToString("F2") + "s";
        }
    }

    public void CheckpointPassed(int index)
    {
        if (index == currentCheckpoint)
        {
            // Split Time
            float timeNow = totalTime;
            float splitTime = timeNow - lastCheckpointTime;
            lastCheckpointTime = timeNow;

            string splitLog = $"Lap {currentLap} - CP {index + 1}: {splitTime:F2}s";
            checkpointSplits.Add(splitLog);

            currentCheckpoint++;
            if (currentCheckpoint >= checkpoints.Length)
                CompleteLap();
        }
    }

    void CompleteLap()
    {
        // Store lap time
        if (currentLap <= lapTimes.Length)
            lapTimes[currentLap - 1] = lapTimer;

        // Stop updating current lap and freeze the text
        if (currentLap == 1)
            lap1Text.text = "Lap 1: " + lapTimer.ToString("F2") + "s";
        else if (currentLap == 2)
            lap2Text.text = "Lap 2: " + lapTimer.ToString("F2") + "s";
        else if (currentLap == 3)
            lap3Text.text = "Lap 3: " + lapTimer.ToString("F2") + "s";

        // Best lap update
        if (lapTimer < sessionBestLap)
        {
            sessionBestLap = lapTimer;
            bestLapText.text = "Best Lap: " + sessionBestLap.ToString("F2") + "s";
        }

        if (lapTimer < bestLapEver)
        {
            bestLapEver = lapTimer;
            PlayerPrefs.SetFloat(BEST_LAP_EVER_KEY, bestLapEver);
            PlayerPrefs.Save();
        }

        // RESET ONLY LAP TIMER (NOT totalTime)
        lapTimer = 0f;
        currentCheckpoint = 0;
        currentLap++;

        if (currentLap > totalLaps)
            FinishRace();
    }

    void FinishRace()
    {
        if (currentRccController != null) currentRccController.enabled = false;

        Rigidbody carRb = playerCar.GetComponent<Rigidbody>();
        if (carRb != null)
        {
            carRb.linearVelocity = Vector3.zero;
            carRb.angularVelocity = Vector3.zero;
        }

        gameUiPanel.SetActive(false);

        if (totalTime < bestOverallEver)
        {
            bestOverallEver = totalTime;
            PlayerPrefs.SetFloat(BEST_OVERALL_KEY, bestOverallEver);
            PlayerPrefs.Save();
        }
       
        // --- GET OFF-TRACK TIME ---
        offTrackTimer = OffTrackTimer.Instance.GetTotalOffTrackTime();
        //if (offTrackTimer != null)
        //{
        //    totalOffTrackTime = offTrackTimer.GetTotalOffTrackTime();
        //}
        // --------------------------

        statsPanel.SetActive(true);

        statsText.text =
            "🏁 Race Complete\n\n" +
            "Lap 1: " + (lapTimes[0] > 0 ? lapTimes[0].ToString("F2") : "--") + "s\n" +
            "Lap 2: " + (lapTimes[1] > 0 ? lapTimes[1].ToString("F2") : "--") + "s\n" +
            "Lap 3: " + (lapTimes[2] > 0 ? lapTimes[2].ToString("F2") : "--") + "s\n\n" +
            "Best Lap: " + sessionBestLap.ToString("F2") + "s\n" +
            "Total Time: " + totalTime.ToString("F2") + "s\n" + // Add Off-Track time here
                "Max Speed: " + maxSpeedReached.ToString("F0") + " km/h\n" +
            "Off Track Time: " + offTrackTimer.ToString("F2") + "s";


        // TELEMETRY LOG
        string logOutput = "\n=== 📊 A4 TELEMETRY DATA ===\n";
        logOutput += $"1. Max Speed Reached: {maxSpeedReached:F2} km/h\n";
        logOutput += $"2. Best Lap Time: {sessionBestLap:F2}s\n";
        logOutput += $"3. Total Off Track Time: {offTrackTimer:F2}s\n"; // Log the off-track time
        logOutput += "4. Checkpoint Splits:\n";

        foreach (string split in checkpointSplits)
            logOutput += "    - " + split + "\n";

        logOutput += "==============================";

        Debug.Log(logOutput);
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}