using UnityEngine;
using UnityEngine.AI;

public class RaceCarAI : MonoBehaviour
{
    public Transform[] waypoints; 
    public float waypointThreshold = 1.0f;  

    private NavMeshAgent agent;
    private int currentWaypoint = 0;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < waypointThreshold)
        {
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
            agent.SetDestination(waypoints[currentWaypoint].position);
        }
    }
}
