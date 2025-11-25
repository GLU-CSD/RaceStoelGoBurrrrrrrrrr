using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            activated = true ;
        }
    }
}
