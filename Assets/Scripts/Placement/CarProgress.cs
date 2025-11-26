using UnityEngine;

public class CarProgress : MonoBehaviour
{
    [SerializeField] private Transform[] checkPoints;

    [SerializeField] private int checkpointIndex = 0;
    [SerializeField] private float checkpointProgress = 0;
    public float totalProgress = 0;

    private void Update()
    {
        float dist = Vector3.Distance(transform.position, checkPoints[checkpointIndex].position);
        checkpointProgress = dist;

        if (dist <= 3f)
        {
            checkpointIndex++;

            if (checkpointIndex >= checkPoints.Length)
            {
                checkpointIndex = 0;
            }
        }

        totalProgress = checkpointIndex * 1000f - dist;
    }
}
