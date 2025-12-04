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

    [Header("Pedal Models")]
    [SerializeField] private Transform gasPedalModel;
    [SerializeField] private Transform brakePedalModel;
    [SerializeField] private Transform pokePedalModel;

    [Header("Pedal Rotation Settings")]
    [SerializeField] private float pedalPressRotation = 20f;  // hoeveel graden een pedaal maximaal draait
    [SerializeField] private float pokeRotation = 25f;         // hoeveel graden de shifter beweegt

    // Jouw echte rustrotaties
    private const float GAS_BRAKE_BASE_X = -154.125f;
    private const float POKE_BASE_X = -89.98f;

    [Header("Movement")]
    [SerializeField] private float maxSpeed = 30f;
    [SerializeField] private float forwardForce = 300f;
    [SerializeField] private float brakeForce = 500f;
    [SerializeField] private float maxSteerAngle = 25f;
    [SerializeField] private float steerSmooth = 5f;
    [SerializeField] private float sideFriction = 0.9f;
    [SerializeField] private float downforce = 50f;
    private float currentSpeed;

    private float currentSteerAngle = 0f;
    private bool isReversing = false;

    private void Start()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0f, 0f);
    }

    private void FixedUpdate()
    {
        float gas = 1f - gasButton.action.ReadValue<float>();
        float pokeValue = poke.action.ReadValue<float>();
        float brake = 1f - brakeButton.action.ReadValue<float>();
        float steerInput = steer.action.ReadValue<float>();

        isReversing = pokeValue > 0.1f;

        UpdateSteeringWheel(steerInput);
        UpdatePedals(gas, brake, pokeValue);

        ApplySteering(steerInput);
        ApplyMovement(gas, brake);
        ApplyStability();
        ApplyDownforce();
        LimitMaxSpeed();
    }

    private void UpdateSteeringWheel(float steerInput)
    {
        if (steeringWheelModel != null)
            steeringWheelModel.localRotation = Quaternion.Euler(0, 0, 180 - steerInput * steeringWheelRotation);
    }

    private void UpdatePedals(float gas, float brake, float pokeValue)
    {
        // Gaspedaal
        if (gasPedalModel != null)
        {
            float rot = GAS_BRAKE_BASE_X + gas * pedalPressRotation;
            gasPedalModel.localRotation = Quaternion.Euler(rot, 0, 0);
        }

        // Rempedaal
        if (brakePedalModel != null)
        {
            float rot = GAS_BRAKE_BASE_X + brake * pedalPressRotation;
            brakePedalModel.localRotation = Quaternion.Euler(rot, 0, 0);
        }

        // Shifter / poke – toggle
        if (pokePedalModel != null)
        {
            float rot = POKE_BASE_X + (pokeValue > 0.1f ? pokeRotation : 0f);
            pokePedalModel.localRotation = Quaternion.Euler(rot, 0, 0);
        }
    }

    private void ApplySteering(float steerInput)
    {
        if (isReversing)
            steerInput *= -1f;

        float speedFactor = Mathf.Clamp(rb.linearVelocity.magnitude / maxSpeed, 0f, 1f);
        float targetSteer = steerInput * maxSteerAngle * speedFactor;

        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, Time.fixedDeltaTime * steerSmooth);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, currentSteerAngle, 0f));
    }

    private void ApplyMovement(float gas, float brake)
    {
        float moveDirection = isReversing ? -1f : 1f;
        currentSpeed = 1f - rb.linearVelocity.magnitude / maxSpeed;
        Vector3 movement = transform.forward * gas * forwardForce * Mathf.Max(currentSpeed, 0.1f) * moveDirection;

        rb.AddForce(movement, ForceMode.Force);

        if (brake > 0.05f)
            rb.AddForce(-rb.linearVelocity.normalized * brake * brakeForce, ForceMode.Force);
    }

    private void ApplyStability()
    {
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
