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

        agent.speed = Random.Range(minSpeed, maxSpeed);
        agent.angularSpeed = Random.Range(160f, 260f);
        agent.acceleration = Random.Range(30f, 60f);
        aggression = Random.Range(0.5f, 2f);
        aiSeed = Random.Range(0f, 999f);

        agent.autoBraking = false;

        if (waypoints.Length > 0)
            SetNewDestination();
    }

    void Update()
    {
        if (waypoints.Length == 0)
            return;

        // Waypoint switch
        if (!agent.pathPending && agent.remainingDistance < waypointThreshold)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            SetNewDestination();
        }

        ApplyWobble();
        TryRamming();
    }

    void SetNewDestination()
    {
        // Look-ahead → betere bochten
        int targetIndex = (currentWaypoint + lookAheadCount) % waypoints.Length;
        Transform baseTarget = waypoints[targetIndex];

        // Offset per auto → elk rijdt anders
        Vector2 offset2D = Random.insideUnitCircle * offsetRange;
        Vector3 offset = new Vector3(offset2D.x, 0, offset2D.y);

        targetPoint = baseTarget.position + offset;
        agent.SetDestination(targetPoint);
    }

    void ApplyWobble()
    {
        Vector3 wobble =
            transform.right *
            Mathf.Sin((Time.time + aiSeed) * wobbleSpeed) *
            wobbleStrength;

        // Veilige side wobble
        agent.Move(wobble * Time.deltaTime);
    }

  

    void TryRamming()
    {
        if (Time.time < lastRamTime + ramCooldown)
            return;

        // Kleine radius om auto's te detecteren die naast hem rijden
        Collider[] nearby = Physics.OverlapSphere(transform.position, 3f);

        foreach (Collider col in nearby)
        {
            if (col.CompareTag("AICar") && col.gameObject != this.gameObject)
            {
                Vector3 dir = (col.transform.position - transform.position).normalized;

                // Alleen rammen als hij naast de andere auto zit
                float sideDot = Vector3.Dot(transform.right, dir);

                if (Mathf.Abs(sideDot) > 0.3f)
                {
                    float force = Mathf.Lerp(200f, ramForce, aggression);
                    rb.AddForce(dir * force);

                    lastRamTime = Time.time;
                    break;
                }
            }
        }
    }
}
