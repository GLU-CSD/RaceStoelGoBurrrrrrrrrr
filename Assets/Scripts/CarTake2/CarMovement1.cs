using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class RealisticCarMovementG29 : MonoBehaviour
{
    [Header("Input Actions")]
    [SerializeField] private InputActionReference gasButton;
    [SerializeField] private InputActionReference poke;       // voor achteruit rijden (toggle)
    [SerializeField] private InputActionReference steer;
    [SerializeField] private InputActionReference brakeButton;

    [Header("Car Settings")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform steeringWheelModel;
    [SerializeField] private float steeringWheelRotation = 450f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float forwardForce = 300f;
    [SerializeField] private float brakeForce = 500f;
    [SerializeField] private float maxSteerAngle = 25f;
    [SerializeField] private float steerSmooth = 5f;
    [SerializeField] private float sideFriction = 0.9f;  // voorkomt sliding
    [SerializeField] private float downforce = 50f;

    private float currentSteerAngle = 0f;

    private bool isReversing = false;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -1f, 0f); // lager zwaartepunt voor stabiliteit
    }

    private void FixedUpdate()
    {
        float gas = 1f - gasButton.action.ReadValue<float>();
        float pokeValue = poke.action.ReadValue<float>();
        float brake = 1f - brakeButton.action.ReadValue<float>();
        float steerInput = steer.action.ReadValue<float>();

        // Check of we in reverse staan
        isReversing = pokeValue > 0.1f;

        // Stuurwiel visueel
        if (steeringWheelModel != null)
            steeringWheelModel.localRotation = Quaternion.Euler(0, 0, 180 - steerInput * steeringWheelRotation);

        ApplySteering(steerInput);
        ApplyMovement(gas, brake);
        ApplyStability();
        ApplyDownforce();
        LimitMaxSpeed();
    }

    private void ApplySteering(float steerInput)
    {
        float speedFactor = Mathf.Clamp(rb.linearVelocity.magnitude / maxSpeed, 0f, 1f);
        float targetSteer = steerInput * maxSteerAngle * speedFactor;
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, Time.fixedDeltaTime * steerSmooth);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, currentSteerAngle, 0f));
    }

    private void ApplyMovement(float gas, float brake)
    {
        // Vooruit of achteruit afhankelijk van isReversing
        float moveDirection = isReversing ? -1f : 1f;
        float speedFactor = 1f - rb.linearVelocity.magnitude / maxSpeed;
        Vector3 movement = transform.forward * gas * forwardForce * Mathf.Max(speedFactor, 0.1f) * moveDirection;
        rb.AddForce(movement, ForceMode.Force);

        // Remmen
        if (brake > 0.05f)
            rb.AddForce(-rb.linearVelocity.normalized * brake * brakeForce, ForceMode.Force);
    }

    private void ApplyStability()
    {
        // Voorkomt dat de auto teveel zijwaarts glijdt
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);
        localVel.x *= sideFriction;
        rb.linearVelocity = transform.TransformDirection(localVel);
    }

    private void ApplyDownforce()
    {
        rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude, ForceMode.Force);
    }

    private void LimitMaxSpeed()
    {
        if (rb.linearVelocity.magnitude > maxSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
    }
}
