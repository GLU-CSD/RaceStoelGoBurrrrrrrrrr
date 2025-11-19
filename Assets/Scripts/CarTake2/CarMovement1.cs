using System.Transactions;
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
    private float pokeValue;
    private float breakValue;

    [Header("Car Movement")]
    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform backLeftWheel;
    [SerializeField] private Transform backRightWheel;
    private float gasValue;
    private float currentGasValue;
    private float carSpeed = 5f;
    private float maxSpeed = 0;

    [Header("Car Steering")]
    private float maxSteerAngle = 30f;
    private float steerSpeed = 8f;
    private float currentSteerAngle;
    private float steerValue;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //Steering Input and movement input
        steerValue = steer.action.ReadValue<float>();
        gasValue = gasButton.action.ReadValue<float>();
        pokeValue = poke.action.ReadValue<float>();
        breakValue = breakButton.action.ReadValue<float>();

        gasValue = 1f - gasValue;
        breakValue = 1f - breakValue;

        Debug.Log(pokeValue);

        //apply steering angle
        float targetSteerAngle = steerValue * maxSteerAngle;

        // apply current move speed
        float targetGasValue = currentGasValue * maxSpeed;

        // smooth steering (car doesn't snap instantly)
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteerAngle, steerSpeed * Time.deltaTime);
        transform.Rotate(0, steerValue * maxSteerAngle * 0.1f, 0);

        //smooth car movement
        currentGasValue = Mathf.Lerp(currentGasValue, targetGasValue, carSpeed * Time.deltaTime);
        if (pokeValue == 0)
        {
            rb.linearVelocity += transform.forward * gasValue / 4;
        }
        else if (pokeValue == 1)
        {
            rb.linearVelocity += transform.forward * gasValue / -4;
        }

        //smooth break

    }
}
