using UnityEngine;
using System.Collections.Generic;

[ExecuteAlways]
public class RaceLineGenerator : MonoBehaviour
{
    [Header("Sampling")]
    public Transform startPoint;
    public LayerMask wallMask;
    public float stepSize = 1f;           // afstand tussen punten
    public float sideRayDistance = 20f;   // afstand naar muren
    public float rayHeight = 1f;          // hoogte van raycasts
    public float groundRayDistance = 5f;  // afstand om weg te volgen

    [Header("Smoothing")]
    public bool smooth = true;
    [Range(0, 5)] public int smoothIterations = 2;

    [Header("Output")]
    [HideInInspector] public List<Vector3> raceLine = new List<Vector3>();
    [HideInInspector] public Vector3[] raceLinePoints;

    [Header("Debug")]
    public bool showGizmos = true;
    public bool drawWallHits = true;
    public Color lineColor = Color.green;
    public Color sampleColor = Color.yellow;

    [ContextMenu("Generate Race Line")]
    public void GenerateRaceLine()
    {
        raceLine.Clear();

        if (startPoint == null) return;

        Vector3 pos = startPoint.position + Vector3.up * rayHeight;
        Vector3 forward = startPoint.forward.normalized;

        Vector3 firstCenter = Vector3.zero;
        bool gotFirst = false;
        int maxPoints = 2000;

        for (int i = 0; i < maxPoints; i++)
        {
            // Raycast links/rechts naar muren
            RaycastHit leftHit, rightHit;
            bool hitLeft = Physics.Raycast(pos, -transform.right, out leftHit, sideRayDistance, wallMask);
            bool hitRight = Physics.Raycast(pos, transform.right, out rightHit, sideRayDistance, wallMask);

            if (!hitLeft) leftHit.point = pos - transform.right * 5f;
            if (!hitRight) rightHit.point = pos + transform.right * 5f;

            // middenpunt
            Vector3 center = (leftHit.point + rightHit.point) * 0.5f;

            // raycast naar weg hoogte
            RaycastHit groundHit;
            if (Physics.Raycast(center + Vector3.up * 5f, Vector3.down, out groundHit, groundRayDistance))
                center.y = groundHit.point.y;
            else
                center.y = startPoint.position.y;

            if (!gotFirst)
            {
                firstCenter = center;
                gotFirst = true;
            }
            else if (Vector3.Distance(center, firstCenter) < stepSize * 0.6f && raceLine.Count > 20)
                break;

            raceLine.Add(center);

            // volgende positie langs tangent
            Vector3 tangent = raceLine.Count >= 2
                ? (raceLine[raceLine.Count - 1] - raceLine[raceLine.Count - 2]).normalized
                : forward;

            if (tangent.sqrMagnitude < 0.0001f)
                tangent = forward;

            pos = center + tangent * stepSize + Vector3.up * rayHeight;
        }

        // smoothing
        if (smooth && raceLine.Count > 3)
        {
            for (int s = 0; s < smoothIterations; s++)
                raceLine = CatmullRomSmooth(raceLine);
        }

        raceLinePoints = raceLine.ToArray();
        Debug.Log($"RaceLineGenerator: generated {raceLinePoints.Length} points.");
    }

    private List<Vector3> CatmullRomSmooth(List<Vector3> pts)
    {
        List<Vector3> sm = new List<Vector3>();
        int n = pts.Count;
        for (int i = 0; i < n; i++)
        {
            Vector3 p0 = pts[(i - 1 + n) % n];
            Vector3 p1 = pts[i];
            Vector3 p2 = pts[(i + 1) % n];
            Vector3 p3 = pts[(i + 2) % n];

            sm.Add(p1);

            int subdivisions = 3;
            for (int s = 1; s <= subdivisions; s++)
            {
                float t = s / (float)(subdivisions + 1);
                sm.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }
        return sm;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;
        return 0.5f * ((2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || raceLine == null || raceLine.Count < 2) return;

        Gizmos.color = lineColor;
        for (int i = 0; i < raceLine.Count; i++)
        {
            Vector3 a = raceLine[i];
            Vector3 b = raceLine[(i + 1) % raceLine.Count];
            Gizmos.DrawLine(a, b);
        }

        if (drawWallHits)
        {
            Gizmos.color = sampleColor;
            foreach (var p in raceLine)
                Gizmos.DrawSphere(p, 0.12f);
        }
    }
}
