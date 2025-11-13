using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    private CharacterController controller;
    public Transform cameraTransform;
    public float gravity = -9.81f;


    [Header("Movement Settings")]
    [SerializeField] private float playerSpeed = 5.0f;
    [SerializeField] private bool isRunning = false;
    public Vector3 move;
    public Vector3 velocity;
    public InputActionReference moveAction;


    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (isRunning == true)
        {
            controller.Move(move * playerSpeed * Time.deltaTime);
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        //Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext ctx)
    {

        //calculate the input of the left joystick
        Vector2 input = ctx.ReadValue<Vector2>();
        Debug.Log(input);
        Vector3 moveInput = new Vector3(input.x, 0, input.y);
        moveInput = Vector3.ClampMagnitude(moveInput, 1f);

        if (moveInput != Vector3.zero)
        {
            //adjust the normal of the player movement to the normal of the camera
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            move = camForward * moveInput.z + camRight * moveInput.x;

            isRunning = true;

        }
        else if (moveInput == Vector3.zero)
        {
            isRunning = false;
        }
    }

}
