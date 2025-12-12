using UnityEngine;

public class VoiceOver : MonoBehaviour
{
    [SerializeField] private AudioSource voiceOver;
    private bool played = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (played == false)
            {
                voiceOver.Play(); 
                played = true;
            }
        }
    }
}
