using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private RaceCarAI[] raceCarAIs;
    [SerializeField] private RealisticCarMovementG29 carMovement;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownTextObject;
    private float countdown = 4;
    private int countdownInt;
    private void Update()
    {
        countdown -= Time.deltaTime;
        countdownInt = (int)countdown;
        countdownText.text = countdownInt.ToString();

        if (countdown <= 0 )
        {
            for(int i = 0; i < raceCarAIs.Length; i++)
            {
                raceCarAIs[i].enabled = true;
            }
            carMovement.enabled = true;
            Destroy(countdownTextObject);
        }
    }
}
