using UnityEngine;

public class SmartRaceCarAI : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 5f;

    public float forwardCheck = 5f;
    public float angleCheck = 4f;
    public float angle = 35f;

    public LayerMask obstacleMask;

    private Rigidbody rb;

    // NEW: smooth steering
    private float targetSteer = 0f;
    private float currentSteer = 0f;
    public float steerSmooth = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        bool frontBlocked = Physics.Raycast(origin, transform.forward, forwardCheck, obstacleMask);

        Vector3 leftDir = Quaternion.AngleAxis(-angle, Vector3.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;

        bool leftSoonBlocked = Physics.Raycast(origin, leftDir, angleCheck, obstacleMask);
        bool rightSoonBlocked = Physics.Raycast(origin, rightDir, angleCheck, obstacleMask);

        float steer = 0f;

        // --------- DECISION LOGIC ---------
        if (frontBlocked)
        {
            if (leftSoonBlocked && !rightSoonBlocked) steer = 1;
            else if (!leftSoonBlocked && rightSoonBlocked) steer = -1;
            else steer = Random.value > 0.5f ? 1 : -1;
        }
        else
        {
            if (leftSoonBlocked && !rightSoonBlocked) steer = 1;
            else if (rightSoonBlocked && !leftSoonBlocked) steer = -1;
            else steer = 0; // no correction needed
        }

        // ---------- SMOOTH STEERING ----------
        targetSteer = steer;
        currentSteer = Mathf.Lerp(currentSteer, targetSteer, Time.fixedDeltaTime * steerSmooth);

        transform.Rotate(0, currentSteer * turnSpeed, 0);

        // move forward
        rb.MovePosition(rb.position + transform.forward * moveSpeed * Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.forward * forwardCheck);

        Vector3 leftDir = Quaternion.AngleAxis(-angle, Vector3.up) * transform.forward;
        Vector3 rightDir = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + leftDir * angleCheck);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + rightDir * angleCheck);
    }
}
