using System.Collections;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioSource priorityAudioSource; // Reference to the higher-priority AudioSource
    public float loopDelay = 5f; // Time in seconds before looping the audio

    private bool isPlayerInTrigger = false; // Tracks if the player is inside the trigger area
    private Coroutine loopCoroutine;

    private void Start()
    {
        // Find the AudioSource component in the child objects
        audioSource = GetComponentInChildren<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in child objects. Please add an AudioSource component.");
        }
        if (priorityAudioSource == null)
        {
            Debug.LogWarning("Priority AudioSource is not assigned. All sounds will play regardless of priority.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is tagged as "Player"
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;

            // Start checking for playback in a coroutine
            if (loopCoroutine == null)
            {
                loopCoroutine = StartCoroutine(LoopAudioWithPriorityCheck());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Stop playing and looping when the player exits the trigger
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            StopLooping();
        }
    }

    private IEnumerator LoopAudioWithPriorityCheck()
    {
        while (isPlayerInTrigger)
        {
            // Stop immediately if priorityAudioSource is playing
            if (priorityAudioSource != null && priorityAudioSource.isPlaying)
            {
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
                StopLooping(); // Stop the coroutine if the priority audio starts playing
                yield break; // Exit the coroutine
            }

            // Play the lower-priority audio if it's not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(loopDelay);
        }
    }

    private void StopLooping()
    {
        if (loopCoroutine != null)
        {
            StopCoroutine(loopCoroutine);
            loopCoroutine = null;
        }
        audioSource.Stop(); // Stop the audio immediately
    }
}
