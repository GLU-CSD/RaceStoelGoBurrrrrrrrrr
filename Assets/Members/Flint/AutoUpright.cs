using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AutoUpright : MonoBehaviour
{
    [Header("Upright Settings")]
    [SerializeField] private float uprightTorque = 10f;   // Hoe snel de auto rechtop komt
    [SerializeField] private float uprightDamping = 0.5f; // Voorkomt trillingen
    [SerializeField] private float maxTiltAngle = 45f;    // Max kanteling in graden

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyUprightStability();
    }

    private void ApplyUprightStability()
    {
        // Bereken torque richting om rechtop te komen
        Vector3 torqueVector = Vector3.Cross(transform.up, Vector3.up);
        rb.AddTorque(torqueVector * uprightTorque - rb.angularVelocity * uprightDamping, ForceMode.Acceleration);

        // Beperk kanteling
        Vector3 flatForward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(flatForward, Vector3.up);

        // Slerp naar target rotation, maar laat kleine kantelingen toe
        rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, uprightTorque * Time.fixedDeltaTime);
    }
}