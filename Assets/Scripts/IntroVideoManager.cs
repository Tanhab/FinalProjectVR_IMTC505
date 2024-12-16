using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class IntroVideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // Reference to VideoPlayer component
    public GameObject xrRig;        // XR Rig GameObject

    private Vector3 initialPosition;  // To lock position
    private Quaternion initialRotation; // To lock rotation

    void Start()
    {
        // Store the XR Rig's initial position and rotation
        initialPosition = xrRig.transform.position;
        initialRotation = xrRig.transform.rotation;

        // Lock the camera rig and prevent movement
        LockPlayerPosition();

        // Subscribe to the video completion event
        videoPlayer.loopPointReached += OnVideoFinished;

        // Play the video
        videoPlayer.Play();
    }

    void Update()
    {
        // Continuously reset the XR Rig's position and rotation to lock it
        xrRig.transform.position = initialPosition;
        xrRig.transform.rotation = initialRotation;
    }

    void LockPlayerPosition()
    {
        // Optionally disable movement input components if present
        var locomotion = xrRig.GetComponent<LocomotionSystem>();
        if (locomotion != null)
        {
            locomotion.enabled = false;
        }
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Video finished, unlock player and load the main scene
        Debug.Log("Video Finished - Loading Main Scene");
        SceneManager.LoadScene("MainScene"); // Replace "MainScene" with your actual scene name
    }
}