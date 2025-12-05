using UnityEngine;

public class MovingCar : MonoBehaviour
{
    public Transform[] movingPoints;
    public int pointCounter = 0;
    public float speed;

    private void Update()
    {
        if (Vector3.Distance(transform.position, movingPoints[pointCounter].position) < 0.02f)
        {
            pointCounter++;
            if (pointCounter >= movingPoints.Length)
            {
                pointCounter = 0;
            }
        }
        transform.position = Vector3.MoveTowards(transform.position, movingPoints[pointCounter].position, speed * Time.deltaTime);
    }
}
