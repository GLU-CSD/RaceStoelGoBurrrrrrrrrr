using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private RaceCarAI[] raceCarAIs;
    [SerializeField] private RealisticCarMovementG29 carMovement;
    [SerializeField] private TextMeshProUGUI countdownText;
    private float countdown = 4;
    private void Update()
    {
        countdown -= Time.deltaTime;
        countdownText.text = countdown.ToString();
        if (countdown <= 0 )
        {
            for(int i = 0; i < raceCarAIs.Length; i++)
            {
                raceCarAIs[i].enabled = true;
            }
            carMovement.enabled = true;
            Destroy(gameObject);
        }
    }
}
