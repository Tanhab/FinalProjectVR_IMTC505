using UnityEngine;

public class XRSetup : MonoBehaviour
{
    public Transform xrRig;      // Reference to XR Rig or Main Camera's parent
    public Vector3 desiredRotation = Vector3.zero; // Default rotation to face the correct direction

    void Start()
    {
        // Reset the XR Rig rotation
        if (xrRig != null)
        {
            xrRig.rotation = Quaternion.Euler(desiredRotation);
            Debug.Log("XR Rig rotation reset to: " + desiredRotation);
        }
    }
}