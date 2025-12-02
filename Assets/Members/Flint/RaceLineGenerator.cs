using UnityEngine;
using System.Collections.Generic;

public class RaceLineGenerator : MonoBehaviour
{
    [Header("Track Sampling")]
    public Transform startPoint;
    public float stepSize = 0.5f;
    public float forwardRayDistance = 3f;
    public float sideRayDistance = 10f;
    public LayerMask trackMask;
    public int maxPoints = 5000;

    [Header("Generated Raceline")]
    public List<Vector3> raceLine = new List<Vector3>();

    [Header("Debug")]
    public bool showGizmos = true;

    void Start()
    {
        GenerateRaceLine();
    }

    public void GenerateRaceLine()
    {
        raceLine.Clear();

        Vector3 currentPos = startPoint.position;
        Vector3 forward = startPoint.forward;

        for (int i = 0; i < maxPoints; i++)
        {
            // Find track boundaries left + right
            Vector3 leftPoint = currentPos - startPoint.right * sideRayDistance;
            Vector3 rightPoint = currentPos + startPoint.right * sideRayDistance;

            RaycastHit hitLeft;
            RaycastHit hitRight;

            Vector3 leftHitPos = currentPos - startPoint.right * 2f;
            Vector3 rightHitPos = currentPos + startPoint.right * 2f;

            if (Physics.Raycast(currentPos, -transform.right, out hitLeft, sideRayDistance, trackMask))
                leftHitPos = hitLeft.point;

            if (Physics.Raycast(currentPos, transform.right, out hitRight, sideRayDistance, trackMask))
                rightHitPos = hitRight.point;

            // Midpoint = raceline
            Vector3 mid = (leftHitPos + rightHitPos) / 2f;
            raceLine.Add(mid);

            // Move forward along track
            RaycastHit forwardHit;
            if (Physics.Raycast(currentPos + Vector3.up, forward, out forwardHit, forwardRayDistance, trackMask))
            {
                currentPos = forwardHit.point + forwardHit.normal * 0.1f;
                forward = Vector3.ProjectOnPlane(forward, forwardHit.normal).normalized;
            }
            else
            {
                // Just move forward
                currentPos += forward * stepSize;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos || raceLine.Count < 2) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < raceLine.Count - 1; i++)
        {
            Gizmos.DrawLine(raceLine[i], raceLine[i + 1]);
        }
    }
}
