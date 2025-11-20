using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement1 : MonoBehaviour
{
    [Header("Input actions")]
    [SerializeField] private InputActionReference gasButton;
    [SerializeField] private InputActionReference steer;
    [SerializeField] private InputActionReference poke;
    [SerializeField] private InputActionReference breakButton;

    [Header("Car Body")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject carBody;

    [Header("Car Wheels")]
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform backLeftWheel;
    [SerializeField] private Transform backRightWheel;

    [Header("Steering Wheel Visual")]
    [SerializeField] private Transform steeringWheelModel;
    [SerializeField] private float steeringWheelRotation = 450f;

    // Movement
    private float gasValue;
    private float steerValue;
    private float pokeValue;
    private float breakValue;

    private float maxSteerAngle = 60f;
    private float carSpeed = 5f;

    // Anti-roll
    [SerializeField] private float antiRollForce = 8000f; // verhoogd

    // Downforce
    [SerializeField] private float downForceStrength = 40f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Lager zwaartepunt = auto valt niet om
        rb.centerOfMass = new Vector3(0f, -1.0f, 0f); // lager dan voorheen
    }

    private void FixedUpdate()
    {
        // Inputs lezen
        steerValue = steer.action.ReadValue<float>();
        gasValue = gasButton.action.ReadValue<float>();
        pokeValue = poke.action.ReadValue<float>();
        breakValue = breakButton.action.ReadValue<float>();

        // G29 pedalen omkeren
        gasValue = 1f - gasValue;
        breakValue = 1f - breakValue;

        // Stuur animatie
        if (steeringWheelModel != null)
        {
            float wheelRot = steerValue * steeringWheelRotation;
            steeringWheelModel.localRotation = Quaternion.Euler(0, 0, 180 - wheelRot);
        }

        // Sturen
        transform.Rotate(0, steerValue * maxSteerAngle * 0.1f, 0f);

        // Vooruit / achteruit
        if (pokeValue == 0)
            rb.linearVelocity += transform.forward * (gasValue / 4f);
        else
            rb.linearVelocity += transform.forward * -(gasValue / 4f);

        // Remmen
        if (breakValue > 0.05f)
        {
            float brakeStrength = 20f;
            Vector3 brakeForce = -rb.linearVelocity.normalized * brakeStrength * breakValue;

            // Niet reverse gaan door remmen
            if (Vector3.Dot(rb.linearVelocity, brakeForce) < 0)
                rb.linearVelocity += brakeForce * Time.fixedDeltaTime;
        }

        // Anti-roll
        ApplyAntiRoll(frontLeftWheel, frontRightWheel);
        ApplyAntiRoll(backLeftWheel, backRightWheel);

        // Minder drift (meer grip op de zijkant)
        float sideGrip = 4f; // hoger = minder drift
        Vector3 localVel = transform.InverseTransformDirection(rb.linearVelocity);

        // Rem de X-snelheid (zijwaarts glijden)
        localVel.x /= (1f + sideGrip * Time.fixedDeltaTime);

        // Terugzetten
        rb.linearVelocity = transform.TransformDirection(localVel);

        // Downforce zodat de auto op de grond blijft
        rb.AddForce(-transform.up * downForceStrength * rb.linearVelocity.magnitude);

        // Extra grip tegen springen
        rb.AddForce(Vector3.down * 5f, ForceMode.Acceleration);
    }

    // Houdt de auto recht tijdens bochten
    private void ApplyAntiRoll(Transform wheelL, Transform wheelR)
    {
        bool groundedL = Physics.Raycast(wheelL.position, -transform.up, out RaycastHit hitL, 0.6f);
        bool groundedR = Physics.Raycast(wheelR.position, -transform.up, out RaycastHit hitR, 0.6f);

        float travelL = groundedL ? (0.6f - hitL.distance) / 0.6f : 1f;
        float travelR = groundedR ? (0.6f - hitR.distance) / 0.6f : 1f;

        float antiRoll = (travelL - travelR) * antiRollForce;

        if (groundedL)
            rb.AddForceAtPosition(-transform.up * antiRoll, wheelL.position);

        if (groundedR)
            rb.AddForceAtPosition(transform.up * antiRoll, wheelR.position);
    }
}
