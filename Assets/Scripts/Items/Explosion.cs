using UnityEngine;

public class Explosion : MonoBehaviour
{
    private float explosionTimer = 1;
    private RaceCarAI raceCarAI;
    private RealisticCarMovementG29 carMovementG29;

    private void Update()
    {
        explosionTimer -= Time.deltaTime;
        if (explosionTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            carMovementG29 = other.GetComponent<RealisticCarMovementG29>();
        }
        else if ( other.gameObject.CompareTag("AICar"))
        {
            raceCarAI = other.GetComponent<RaceCarAI>();
        }
    }
}
