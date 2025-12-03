using UnityEngine;
using UnityEngine.InputSystem;

public class EngineMultiLayerEngine : MonoBehaviour
{
    [Header("RPM Settings")]
    public float idleRPM = 1000f;
    public float maxRPM = 9000f;
    public float currentRPM = 1000f;
    public float rpmIncreaseRate = 5000f;      // how fast RPM rises
    public float rpmDecreaseRate = 3000f;      // how fast RPM falls

    [Header("Gears")]
    public int gear = 1;
    public int maxGear = 6;
    public float[] gearRatios = { 2.8f, 2.1f, 1.6f, 1.3f, 1.1f, 0.9f };
    public float shiftUpRPM = 8000f;
    public float shiftDownRPM = 1500f;
    public float shiftCooldown = 0.5f;
    private float lastShiftTime;

    [Header("Audio (17 clips + 17 sources)")]
    public AudioClip[] rpmClips;           // 17 engine samples
    public AudioSource[] audioSources;     // 17 AudioSources
    public float rpmStep = 500f;
    public float fadeSpeed = 8f;

    private float[] clipRPMs;

    [Header("Input")]
    public bool useNewInputSystem = true;

    // ⭐️ REPLACED WITH InputActionReference AS REQUESTED ⭐️
    [SerializeField] private InputActionReference throttleAction;

    private void OnEnable()
    {
        if (useNewInputSystem && throttleAction != null)
            throttleAction.action.Enable();
    }

    private void OnDisable()
    {
        if (useNewInputSystem && throttleAction != null)
            throttleAction.action.Disable();
    }

    void Start()
    {
        clipRPMs = new float[rpmClips.Length];

        for (int i = 0; i < rpmClips.Length; i++)
        {
            clipRPMs[i] = idleRPM + i * rpmStep;

            audioSources[i].clip = rpmClips[i];
            audioSources[i].loop = true;
            audioSources[i].volume = 0f;
            audioSources[i].pitch = 1f;
            audioSources[i].Play();
        }
    }

    void Update()
    {
        float throttle = GetThrottle();

        UpdateRPM(throttle);
        UpdateGears();
        UpdateEngineLayers();
    }

    // -------------------------------
    // INPUT HANDLING
    // -------------------------------
    float GetThrottle()
    {
        if (useNewInputSystem)
        {
            if (throttleAction == null || throttleAction.action == null)
                return 0f;

            return throttleAction.action.ReadValue<float>();
        }
        else
        {
            return Input.GetKey(KeyCode.W) ? 1f : 0f;
        }
    }

    // -------------------------------
    // RPM SIMULATION
    // -------------------------------
    void UpdateRPM(float throttle)
    {
        if (throttle > 0)
            currentRPM += throttle * rpmIncreaseRate * Time.deltaTime * gearRatios[gear - 1];
        else
            currentRPM -= rpmDecreaseRate * Time.deltaTime;

        currentRPM = Mathf.Clamp(currentRPM, idleRPM, maxRPM);
    }

    // -------------------------------
    // AUTO TRANSMISSION
    // -------------------------------
    void UpdateGears()
    {
        if (Time.time < lastShiftTime + shiftCooldown) return;

        if (currentRPM > shiftUpRPM && gear < maxGear)
        {
            gear++;
            currentRPM *= 0.6f;  // RPM drop
            lastShiftTime = Time.time;
        }
        else if (currentRPM < shiftDownRPM && gear > 1)
        {
            gear--;
            currentRPM *= 1.3f;
            lastShiftTime = Time.time;
        }
    }

    // -------------------------------
    // ENGINE SOUND LAYER SYSTEM
    // -------------------------------
    void UpdateEngineLayers()
    {
        for (int i = 0; i < audioSources.Length; i++)
        {
            float clipRPM = clipRPMs[i];
            float rpmDifference = Mathf.Abs(currentRPM - clipRPM);

            float targetVolume = (rpmDifference < rpmStep)
                ? rpmStep / (rpmDifference + rpmStep)
                : 0f;

            audioSources[i].volume = Mathf.Lerp(
                audioSources[i].volume,
                targetVolume,
                Time.deltaTime * fadeSpeed
            );

            float targetPitch = currentRPM / clipRPM;
            audioSources[i].pitch = Mathf.Lerp(
                audioSources[i].pitch,
                targetPitch,
                Time.deltaTime * fadeSpeed
            );
        }
    }
}
