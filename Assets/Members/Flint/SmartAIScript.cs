using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SmartRaceCarAI : MonoBehaviour
{
    public RaceLineGenerator lineGenerator;
    public Transform player;
    public float baseSpeed = 20f;
    public float lookAhead = 6f; // kleiner voor stabielere bochten
    public float turnSpeed = 5f;
    public float offsetRange = 0.8f; // smaller offset
    public float rubberStrength = 0.5f;
    public float maxBoost = 1.6f;
    public float maxSlow = 0.6f;

    private Rigidbody rb;
    private Vector3[] raceLine;
    private int index = 0;
    private float offset;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (lineGenerator == null) return;

        lineGenerator.GenerateRaceLine();
        raceLine = lineGenerator.raceLinePoints;

        offset = Random.Range(-offsetRange, offsetRange);
    }

    void FixedUpdate()
    {
        if (raceLine == null || raceLine.Length == 0) return;

        index = GetClosestIndex();
        float lookIndex = (index + lookAhead) % raceLine.Length;

        Vector3 target = GetInterpolatedPoint(lookIndex) + transform.right * offset;

        Steer(target);

        float speed = ApplyRubberBanding(baseSpeed);
        ApplyDriving(target, speed);
    }

    void Steer(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.fixedDeltaTime * turnSpeed);
    }

    void ApplyDriving(Vector3 target, float speed)
    {
        float curve = Vector3.Angle(transform.forward, (target - transform.position).normalized);
        float speedMultiplier = Mathf.Lerp(1f, 0.4f, Mathf.InverseLerp(0, 90, curve));

        rb.AddForce(transform.forward * speed * speedMultiplier, ForceMode.Acceleration);
    }

    float ApplyRubberBanding(float speed)
    {
        if (player == null) return speed;

        int aiProgress = index;
        int playerProgress = GetClosestIndexTo(player.position);

        float delta = Mathf.DeltaAngle(playerProgress, aiProgress);

        if (delta > 10f)
        {
            float t = Mathf.Clamp01(delta / 100f) * rubberStrength;
            return speed * Mathf.Lerp(1f, maxBoost, t);
        }
        else if (delta < -10f)
        {
            float t = Mathf.Clamp01(-delta / 100f) * rubberStrength;
            return speed * Mathf.Lerp(1f, maxSlow, t);
        }

        return speed;
    }

    int GetClosestIndex() => GetClosestIndexTo(transform.position);

    int GetClosestIndexTo(Vector3 pos)
    {
        int best = 0;
        float bestDist = Mathf.Infinity;
        for (int i = 0; i < raceLine.Length; i++)
        {
            float d = (raceLine[i] - pos).sqrMagnitude;
            if (d < bestDist)
            {
                bestDist = d;
                best = i;
            }
        }
        return best;
    }

    Vector3 GetInterpolatedPoint(float t)
    {
        int i0 = Mathf.FloorToInt(t) % raceLine.Length;
        int i1 = (i0 + 1) % raceLine.Length;
        float f = t - Mathf.Floor(t);
        return Vector3.Lerp(raceLine[i0], raceLine[i1], f);
    }
}
