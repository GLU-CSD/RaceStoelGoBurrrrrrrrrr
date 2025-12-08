using UnityEngine;

public class SmartRaceCarAI : MonoBehaviour
{
    [Header("Checkpoints")]
    public Transform[] checkpoints;
    public float checkpointReachDistance = 5f;
    private int currentCheckpoint = 0;

    [Header("Driving")]
    public float speedMin = 10f;
    public float speedMax = 14f;
    private float speed;
    public float turnSpeed = 5f;

    [Header("Obstacle Avoidance")]
    public LayerMask wallLayer;
    public LayerMask carLayer;
    public LayerMask playerLayer;
    public float forwardDistance = 6f; // forward ray distance
    public float sideDistance = 4f;    // side ray distance
    [Range(0f, 1f)]
    public float avoidanceBlend = 0.6f;

    [Header("Variation")]
    public float lateralOffsetMax = 1f;
    private float offset;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        speed = Random.Range(speedMin, speedMax);
        offset = Random.Range(-lateralOffsetMax, lateralOffsetMax);
    }

    void FixedUpdate()
    {
        if (checkpoints.Length == 0 || currentCheckpoint >= checkpoints.Length) return;

        Vector3 target = checkpoints[currentCheckpoint].position + transform.right * offset;
        Vector3 dirToCheckpoint = (target - transform.position).normalized;

        Vector3 finalDir = dirToCheckpoint;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        RaycastHit hit;

        // --- Forward check ---
        bool forwardBlocked = Physics.Raycast(origin, transform.forward, out hit, forwardDistance, wallLayer | carLayer | playerLayer);

        // --- Side checks (6 rays) ---
        float[] sideAngles = { -60f, -30f, -10f, 10f, 30f, 60f };
        Vector3 bestDir = Vector3.zero;
        float maxFreeDist = -1f;

        foreach (float angle in sideAngles)
        {
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            float freeDistance = sideDistance;

            if (Physics.Raycast(origin, dir, out hit, sideDistance, wallLayer | carLayer | playerLayer))
            {
                freeDistance = hit.distance;
            }

            if (freeDistance > maxFreeDist)
            {
                maxFreeDist = freeDistance;
                bestDir = dir;
            }
        }

        // --- Kies richting met meeste vrije ruimte ---
        if (forwardBlocked)
        {
            finalDir = bestDir.normalized;
        }

        // --- Blend met checkpoint richting ---
        finalDir = ((dirToCheckpoint * (1 - avoidanceBlend)) + (finalDir * avoidanceBlend)).normalized;

        // --- Steering ---
        if (finalDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(finalDir);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }

        // --- Move forward ---
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);

        // --- Next checkpoint ---
        if (Vector3.Distance(transform.position, target) < checkpointReachDistance)
            currentCheckpoint++;
    }

    void OnDrawGizmos()
    {
        if (checkpoints == null || checkpoints.Length == 0) return;

        // Checkpoint lines
        Gizmos.color = Color.yellow;
        for (int i = 0; i < checkpoints.Length - 1; i++)
        {
            if (checkpoints[i] != null && checkpoints[i + 1] != null)
                Gizmos.DrawLine(checkpoints[i].position, checkpoints[i + 1].position);
        }

        // Forward ray
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.forward * forwardDistance);

        // Side rays
        Gizmos.color = Color.cyan;
        float[] sideAngles = { -60f, -30f, -10f, 10f, 30f, 60f };
        foreach (float angle in sideAngles)
        {
            Vector3 dir = Quaternion.Euler(0, angle, 0) * transform.forward;
            Gizmos.DrawLine(origin, origin + dir * sideDistance);
        }
    }
}
