using UnityEngine;
using UnityEngine.AI;

public class RaceCarAI : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;
    public float waypointThreshold = 3f;

    [Header("Driving Variation")]
    public float offsetRange = 2f;
    public float wobbleStrength = 0.5f;
    public float wobbleSpeed = 1.5f;

    [Header("AI Personality")]
    public float minSpeed = 8f;
    public float maxSpeed = 12f;

    [Header("Lookahead")]
    public int lookAheadCount = 2;

    [Header("Overtake + Attack")]
    public float detectDistance = 8f;
    public float ramForce = 500f;
    public float ramCooldown = 1.5f;
    public float aggression = 0.5f;

    private float lastRamTime = 0f;

    private NavMeshAgent agent;
    private int currentWaypoint = 0;
    private Vector3 targetPoint;
    private float aiSeed;
    private Rigidbody rb;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        // Random rijgedrag
        agent.speed = Random.Range(minSpeed, maxSpeed);
        agent.angularSpeed = Random.Range(160f, 260f);
        agent.acceleration = Random.Range(30f, 60f);
        aggression = Random.Range(0.5f, 2f);
        aiSeed = Random.Range(0f, 999f);

        agent.autoBraking = false;
        agent.autoRepath = false;

        if (waypoints.Length > 0)
            SetNewDestination();
    }

    void Update()
    {
        if (waypoints.Length == 0)
            return;

        HandleWaypointSwitching();
        ApplyWobble();
        TryRamming();
    }

    void HandleWaypointSwitching()
    {
        // Nog onderweg
        if (agent.pathPending || agent.remainingDistance >= waypointThreshold)
            return;

        // Richting naar waypoint
        Vector3 toWaypoint = waypoints[currentWaypoint].position - transform.position;
        float dot = Vector3.Dot(transform.forward, toWaypoint.normalized);

        // Waypoint achter de auto → overslaan (geen U-turn)
        if (dot < 0f)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            SetNewDestination();
            return;
        }

        // Normaal naar volgende
        currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        SetNewDestination();
    }

    void SetNewDestination()
    {
        int targetIndex = currentWaypoint;

        // Zoek vooruit-liggend waypoint
        for (int i = 0; i < lookAheadCount; i++)
        {
            int next = (currentWaypoint + i) % waypoints.Length;
            Vector3 dir = waypoints[next].position - transform.position;

            if (Vector3.Dot(transform.forward, dir.normalized) > 0f)
            {
                targetIndex = next;
                break;
            }
        }

        // Klein random offset
        Vector2 offset2D = Random.insideUnitCircle * offsetRange;
        Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);

        targetPoint = waypoints[targetIndex].position + offset;
        agent.SetDestination(targetPoint);
    }

    void ApplyWobble()
    {
        // Kleine zijwaartse beweging
        Vector3 wobble =
            transform.right *
            Mathf.Sin((Time.time + aiSeed) * wobbleSpeed) *
            wobbleStrength;

        agent.Move(wobble * Time.deltaTime);
    }

    void TryRamming()
    {
        if (Time.time < lastRamTime + ramCooldown)
            return;

        Collider[] nearby = Physics.OverlapSphere(transform.position, 3f);

        foreach (Collider col in nearby)
        {
            if (col.CompareTag("AICar") && col.gameObject != this.gameObject)
            {
                Vector3 dir = (col.transform.position - transform.position).normalized;
                float sideDot = Vector3.Dot(transform.right, dir);

                // Alleen rammen als de auto naast hem zit
                if (Mathf.Abs(sideDot) > 0.3f)
                {
                    float force = Mathf.Lerp(200f, ramForce, aggression);
                    rb.AddForce(dir * force, ForceMode.Impulse);

                    lastRamTime = Time.time;
                    break;
                }
            }
        }
    }
}
