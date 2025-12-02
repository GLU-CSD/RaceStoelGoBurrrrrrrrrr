using UnityEngine;
using UnityEngine.InputSystem;

public class CarEngineSoundAutoShift : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource engineAudio;
    public AudioSource shiftAudio; // optional

    [Header("Pitch Settings")]
    public float idlePitch = 0.35f;       // deeper idle
    public float redlinePitch = 2.3f;     // enough room to rev
    public float pitchLerpSpeed = 6f;

    [Header("Gear Settings")]
    public int totalGears = 5;

    // LONG GEARS → SLOWER RPM CLIMB
    public float rpmIncreaseSpeed = 0.6f;   // was ~1.6, reduced a LOT
    public float rpmDecreaseSpeed = 0.9f;

    // Shift points (stay high because gears are long)
    public float shiftUpRPM = 0.95f;
    public float shiftDownRPM = 0.2f;

    private int currentGear = 1;
    private float rpm = 0f;
    private float targetPitch;

    private bool shiftDropping = false;
    private float shiftDropTimer = 0f;

    void Update()
    {
        bool accelerating = Keyboard.current != null && Keyboard.current.wKey.isPressed;

        // ==============================
        // NORMAL RPM BUILD/DROP
        // ==============================
        if (!shiftDropping)
        {
            if (accelerating)
                rpm += Time.deltaTime * rpmIncreaseSpeed;  // slower build
            else
                rpm -= Time.deltaTime * rpmDecreaseSpeed;

            rpm = Mathf.Clamp01(rpm);
        }

        // ==============================
        // SHIFT DROP
        // ==============================
        if (shiftDropping)
        {
            shiftDropTimer -= Time.deltaTime;

            // HUGE DROP — sounds like big gear change
            rpm = Mathf.Lerp(rpm, 0.15f, Time.deltaTime * 14f);

            // End shift dip
            if (shiftDropTimer <= 0)
                shiftDropping = false;
        }

        // ==============================
        // AUTO SHIFT UP
        // ==============================
        if (!shiftDropping && rpm >= shiftUpRPM && currentGear < totalGears)
        {
            currentGear++;

            if (shiftAudio != null)
                shiftAudio.Play();

            shiftDropping = true;
            shiftDropTimer = 0.28f;    // LONGER dip for dramatic down-blip
        }

        // ==============================
        // AUTO SHIFT DOWN
        // ==============================
        if (!shiftDropping && rpm <= shiftDownRPM && currentGear > 1)
        {
            currentGear--;
            rpm = 0.7f; 
        }

        // ==============================
        // PITCH CALCULATION
        // ==============================
        float gearBasePitch = idlePitch + (currentGear - 1) * 0.12f;  
        // Slightly smaller gear pitch increase so long gears don't stack pitch too high

        // RPM adds rev sound on top of gear base
        float rpmPitch = Mathf.Lerp(0f, redlinePitch - gearBasePitch, rpm);

        targetPitch = gearBasePitch + rpmPitch;

        // Smooth pitch transition
        engineAudio.pitch = Mathf.Lerp(engineAudio.pitch, targetPitch, Time.deltaTime * pitchLerpSpeed);
    }
}
