using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class EngineSoundSystem : MonoBehaviour
{
    [Header("RPM Settings")]
    public float minRPM = 1000f;
    public float maxRPM = 9000f;
    public float currentRPM = 1000f;
    public float rpmSmoothing = 5f;

    [Header("Input (New Input System)")]
    public InputAction throttleInput; // 0 = no gas, 1 = full gas

    [Header("Gears")]
    public int currentGear = 1;
    public float[] gearRatios = { 2.8f, 2.1f, 1.6f, 1.3f, 1.1f, 0.9f };
    public float shiftCooldown = 0.6f;
    private float lastShiftTime;

    [Header("Audio")]
    public AudioSource sourceA;
    public AudioSource sourceB;
    public AudioClip[] rpmClips;

    private bool usingA = true;
    private int lastClipIndex = -1;

    private void OnEnable()
    {
        throttleInput.Enable();
    }

    private void OnDisable()
    {
        throttleInput.Disable();
    }

    private void Start()
    {
        // Start both audio sources muted
        sourceA.volume = 0f;
        sourceB.volume = 0f;

        // Play initial clip to avoid silence
        if (rpmClips.Length > 0)
        {
            sourceA.clip = rpmClips[0];
            sourceA.Play();
        }
    }

    private void Update()
    {
        UpdateRPM();
        UpdateEngineSound();
    }

    private void UpdateRPM()
    {
        float throttle = throttleInput.ReadValue<float>(); // NEW INPUT SYSTEM

        // Target RPM based on throttle
        float targetRPM = Mathf.Lerp(minRPM, maxRPM, throttle);

        // Apply gear ratio
        targetRPM *= gearRatios[currentGear - 1];

        // Clamp
        targetRPM = Mathf.Clamp(targetRPM, minRPM, maxRPM);

        // Smooth RPM movement
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime * rpmSmoothing);

        // --- Auto Shifting ---
        if (Time.time > lastShiftTime + shiftCooldown)
        {
            // Upshift (when near redline)
            if (currentRPM > maxRPM * 0.92f && currentGear < gearRatios.Length)
            {
                currentGear++;
                currentRPM *= 0.55f;  // RPM drop for realism
                lastShiftTime = Time.time;
            }
            // Downshift (if RPM too low)
            else if (currentRPM < minRPM * 1.15f && currentGear > 1)
            {
                currentGear--;
                currentRPM *= 1.45f;
                lastShiftTime = Time.time;
            }
        }
    }

    private void UpdateEngineSound()
    {
        if (rpmClips == null || rpmClips.Length == 0)
            return;

        float normalized = (currentRPM - minRPM) / (maxRPM - minRPM);
        normalized = Mathf.Clamp01(normalized);

        // determine which clip to use (0–16)
        int clipIndex = Mathf.FloorToInt(normalized * (rpmClips.Length - 1));
        clipIndex = Mathf.Clamp(clipIndex, 0, rpmClips.Length - 1);

        if (clipIndex == lastClipIndex)
            return;

        // Select audio sources
        AudioSource next = usingA ? sourceB : sourceA;
        AudioSource current = usingA ? sourceA : sourceB;

        next.clip = rpmClips[clipIndex];
        next.volume = 0f;
        next.Play();

        StartCoroutine(Crossfade(current, next));

        usingA = !usingA;
        lastClipIndex = clipIndex;
    }

    private IEnumerator Crossfade(AudioSource from, AudioSource to)
    {
        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * 5f; // fade speed (adjustable)
            from.volume = Mathf.Lerp(1f, 0f, t);
            to.volume = Mathf.Lerp(0f, 1f, t);
            yield return null;
        }

        from.Stop();
    }
}
