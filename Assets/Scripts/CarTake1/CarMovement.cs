using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarMovement : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference gasButton;
    [SerializeField] private InputActionReference breakButton;
    [SerializeField] private InputActionReference turnLeftButton;
    [SerializeField] private InputActionReference turnRightButton;
    [SerializeField] private InputActionReference driftButton;

    [Header("Wheels")]
    [SerializeField] private WheelCollider frontLeft;
    [SerializeField] private WheelCollider frontRight;
    [SerializeField] private WheelCollider backLeft;
    [SerializeField] private WheelCollider backRight;

    [Header("Settings")]
    private float acceleration = 800f;
    private float brakingForce = 300f;
    private float maxTurnAngle = 20f;
    private float steerSmoothSpeed = 5f; // smooth steering

    private float currentAcceleration = 0f;
    private float currentBrakeForce = 0f;
    private float currentSteerAngle = 0f;


    private void FixedUpdate()
    {
        // --- ACCELERATION ---
        if (gasButton.action.IsPressed())
            currentAcceleration = acceleration;
        else
            currentAcceleration = 0f;

        // --- BRAKING ---
        if (breakButton.action.IsPressed())
            currentBrakeForce = brakingForce;
        else
            currentBrakeForce = 0f;

        // --- DRIFTING ---
        if (driftButton.action.IsPressed())
        {
            backLeft.steerAngle = -currentSteerAngle - 10;
            backRight.steerAngle = -currentSteerAngle - 10;
        }
        else
        {
            backLeft.steerAngle = 0;
            backRight.steerAngle = 0;
        }


        // --- STEERING ---
        float targetSteer = 0f;
        if (turnLeftButton.action.IsPressed()) targetSteer = -maxTurnAngle;
        else if (turnRightButton.action.IsPressed()) targetSteer = maxTurnAngle;

        // Smooth steering transition
        currentSteerAngle = Mathf.Lerp(currentSteerAngle, targetSteer, Time.fixedDeltaTime * steerSmoothSpeed);

        // --- APPLY TO WHEELS ---
        frontLeft.motorTorque = currentAcceleration;
        frontRight.motorTorque = currentAcceleration;

        frontLeft.steerAngle = currentSteerAngle;
        frontRight.steerAngle = currentSteerAngle;

        frontLeft.brakeTorque = currentBrakeForce;
        frontRight.brakeTorque = currentBrakeForce;
        backLeft.brakeTorque = currentBrakeForce;
        backRight.brakeTorque = currentBrakeForce;
    }
}