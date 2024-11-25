using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasRotetesToPlayer : MonoBehaviour
{
    public Transform playerCamera; // Reference to the main camera or player's head position
    public Vector3 offset = new Vector3(0.5f, 0, 0); // Offset to the right of the animal

    private void Update()
    {
        if (playerCamera == null) return;

        // Make the canvas face the camera
        Vector3 direction = (transform.position - playerCamera.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Apply the offset relative to the canvas's forward direction
        transform.position = transform.parent.position + transform.right * offset.x + transform.up * offset.y + transform.forward * offset.z;
    }
}
