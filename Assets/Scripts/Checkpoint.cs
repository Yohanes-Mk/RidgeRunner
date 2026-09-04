using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex;
    public LapManager lapManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // tag your car as "Player"
        {
            lapManager.CheckpointPassed(checkpointIndex);
        }
    }
}
