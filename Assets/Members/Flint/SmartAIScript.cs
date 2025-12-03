using UnityEngine;

public class SmartRaceCarAI : MonoBehaviour
{
    public RaceLineGenerator lineGenerator;
    private Vector3[] raceLine;

    [Header("Driving")]
    public float baseSpeed = 20f;
    public float turnSpeed = 5f;
    public float lookAhead = 12f;

    [Header("Variation")]
    public float offset;

    [Header("Rubber Banding")]
    public Transform player;
    public float rubberStrength = 0.5f;  // 0 = uit, 1 = extreem
    public float maxBoost = 1.6f;        // max 160% snelheid
    public float maxSlow = 0.6f;         // min 60% snelheid

    private Rigidbody rb;
    private int index = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        raceLine = lineGenerator.raceLine.ToArray();

        offset = Random.Range(-2f, 2f);
    }

    void FixedUpdate()
    {
        if (raceLine.Length == 0) return;

        index = GetClosestIndex();

        int lookIndex = (index + Mathf.RoundToInt(lookAhead)) % raceLine.Length;

        Vector3 target = raceLine[lookIndex] + transform.right * offset;

        Steer(target);

        float rbSpeed = ApplyRubberBanding(baseSpeed);

        rb.AddForce(transform.forward * rbSpeed, ForceMode.Acceleration);
    }

    void Steer(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        Quaternion rot = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.fixedDeltaTime * turnSpeed);
    }

    float ApplyRubberBanding(float speed)
    {
        if (player == null) return speed;

        int aiProgress = index;
        int playerProgress = GetClosestIndexTo(player.position);

        int delta = playerProgress - aiProgress;

        // AI staat achter → BOOST
        if (delta > 10)
        {
            float t = Mathf.Clamp01(delta / 100f) * rubberStrength;
            return speed * Mathf.Lerp(1f, maxBoost, t);
        }

        // AI staat ver voor → AFREMMEN
        if (delta < -10)
        {
            float t = Mathf.Clamp01(-delta / 100f) * rubberStrength;
            return speed * Mathf.Lerp(1f, maxSlow, t);
        }

        return speed;
    }

    int GetClosestIndex()
    {
        return GetClosestIndexTo(transform.position);
    }

    int GetClosestIndexTo(Vector3 pos)
    {
        int bestIndex = 0;
        float bestDist = Mathf.Infinity;

        for (int i = 0; i < raceLine.Length; i++)
        {
            float dist = (raceLine[i] - pos).sqrMagnitude;
            if (dist < bestDist)
            {
                bestDist = dist;
                bestIndex = i;
            }
        }

        return bestIndex;
    }
}
