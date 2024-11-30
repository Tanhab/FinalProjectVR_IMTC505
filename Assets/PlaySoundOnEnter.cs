using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnTrigger : MonoBehaviour
{
    private AudioSource audioSource;

    private void Start()
    {
        // Find the AudioSource component in the child objects
        audioSource = GetComponentInChildren<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found in child objects. Please add an AudioSource component.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is tagged as "Player" or any other tag you define
        if (other.CompareTag("Player") && audioSource != null)
        {
            // Play the sound
            audioSource.Play();
        }
    }
}