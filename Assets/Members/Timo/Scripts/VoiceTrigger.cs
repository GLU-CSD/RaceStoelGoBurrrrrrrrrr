using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;    // Assign the AudioSource that will play the voice
    public AudioClip voiceLine;        // Assign the voice line for THIS trigger

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;                          // Already used
        if (!other.CompareTag("Player")) return;        // Only the Player triggers it

        hasPlayed = true;                               // Mark as used
        audioSource.PlayOneShot(voiceLine);             // Play the voice line
    }
}
