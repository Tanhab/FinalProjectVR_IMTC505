using UnityEngine;

public class CanvasRotatesToPlayer : MonoBehaviour
{
    public Transform playerCamera; // Reference to the main camera or player's head position

    private void Update()
    {
        if (playerCamera == null) return;

        // Get the direction from the canvas to the camera, but ignore the Y-axis
        Vector3 direction = playerCamera.position - transform.position;
        direction.y = 0; // Keep the canvas height unchanged

        // Rotate the canvas to face the player camera
        Quaternion lookRotation = Quaternion.LookRotation(-direction);
        transform.rotation = lookRotation;
    }
}