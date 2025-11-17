using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement1 : MonoBehaviour
{
    [SerializeField] private InputActionReference gasButton;
    [SerializeField] private InputActionReference reverseButton;
    [SerializeField] private InputActionReference turnLeftButton;
    [SerializeField] private InputActionReference turnRightButton;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject carBody;

    [SerializeField] private Transform frontLeftWheel;
    [SerializeField] private Transform frontRightWheel;
    [SerializeField] private Transform backLeftWheel;
    [SerializeField] private Transform backRightWheel;

    [SerializeField] private Transform left;
    [SerializeField] private Transform right;
    [SerializeField] private Transform straight;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (gasButton.action.IsPressed())
        {
            rb.linearVelocity += transform.forward * 20 * Time.deltaTime;

            frontLeftWheel.Rotate(0, 0, -200 * Time.deltaTime);
            frontRightWheel.Rotate(0, 0, -200 * Time.deltaTime);
            backLeftWheel.Rotate(0, 0, -200 * Time.deltaTime);
            backRightWheel.Rotate(0, 0, -200 * Time.deltaTime);
        }

        if (reverseButton.action.IsPressed())
        {
            rb.linearVelocity -= transform.forward * 80 * Time.deltaTime;

            frontLeftWheel.Rotate(0, 0, 200 * Time.deltaTime);
            frontRightWheel.Rotate(0, 0, 200 * Time.deltaTime);
            backLeftWheel.Rotate(0, 0, 200 * Time.deltaTime);
            backRightWheel.Rotate(0, 0, 200 * Time.deltaTime);
        }

        if (turnLeftButton.action.IsPressed())
        {
            transform.Rotate(0, -80 * Time.deltaTime, 0);
            carBody.transform.rotation = Quaternion.Lerp(carBody.transform.rotation, left.rotation, 4 * Time.deltaTime);
            rb.angularVelocity += carBody.transform.forward * 120 * Time.deltaTime;
            rb.angularVelocity -= transform.forward *110 * Time.deltaTime;
        }

        if (turnRightButton.action.IsPressed())
        {
            transform.Rotate(0, 80 * Time.deltaTime, 0);
            carBody.transform.rotation = Quaternion.Lerp(carBody.transform.rotation, right.rotation, 4 * Time.deltaTime);
            rb.angularVelocity += transform.forward * 120 * Time.deltaTime;
            rb.angularVelocity -= carBody.transform.forward * 110 * Time.deltaTime;
        }
    }
}
