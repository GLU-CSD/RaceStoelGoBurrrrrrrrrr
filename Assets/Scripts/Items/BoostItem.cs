using UnityEngine;

public class BoostItem : MonoBehaviour
{
    private RealisticCarMovementG29 carMovementG29;
    private float boostTime = 3;
    private void Start()
    {
        carMovementG29 = GameObject.FindGameObjectWithTag("Player").GetComponent<RealisticCarMovementG29>();
    }
    private void Update()
    {
        boostTime -= Time.deltaTime;
        if (boostTime >= 0)
        {
            carMovementG29.maxSpeed = 50;
        }
        else
        {
            carMovementG29.maxSpeed = 30;
        }
    }
}
