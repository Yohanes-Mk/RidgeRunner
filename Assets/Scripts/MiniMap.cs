using UnityEngine;

public class MiniMap : MonoBehaviour
{
    // Make these public properties (or use [HideInInspector]) so the CarSelector can assign them.
    [HideInInspector] public Transform _playerTransform;
    [HideInInspector] public Transform _playerIcon;
    private float _playerIconOffset = 1.5f;

    // 🌟 NEW METHOD: Called by CarSelector after the car is spawned.
    public void SetPlayerTarget(Transform playerCar, Transform playerIcon)
    {
        _playerTransform = playerCar;
        _playerIcon = playerIcon;
    }

    // You can remove the original Start() method entirely.

    void LateUpdate() // Use LateUpdate for camera/minimap following logic
    {
        // Check if both targets have been assigned before attempting to follow.
        if (_playerTransform != null && _playerIcon != null)
        {
            // Match the sprite's position to the player's position
            _playerIcon.position = new Vector3(
                _playerTransform.position.x,
                transform.position.y - _playerIconOffset,
                _playerTransform.position.z
            );

            // Calculate the desired rotation for the player icon (90 degrees on X for top-down view)
            Quaternion desiredRotation = Quaternion.Euler(90f, _playerTransform.eulerAngles.y, 0f);

            // Match the player icon's rotation
            _playerIcon.rotation = desiredRotation;
        }
    }
}