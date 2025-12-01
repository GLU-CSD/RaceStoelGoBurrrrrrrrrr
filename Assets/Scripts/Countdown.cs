using TMPro;
using UnityEngine;

public class Countdown : MonoBehaviour
{
    [SerializeField] private RaceCarAI[] raceCarAIs;
    [SerializeField] private RealisticCarMovementG29 carMovement;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GameObject countdownTextObject;
    [SerializeField] private AudioSource abandonShip;
    private float countdown = 4.3f;
    private int countdownInt;
    private void Update()
    {
        countdown -= Time.deltaTime;
        countdownInt = (int)countdown;
        if (countdown <= 3)
        {
            countdownText.text = countdownInt.ToString();
        }
        else
        {
            countdownText.text = "3";
        }

        if (countdown <= 0)
        {
            abandonShip.enabled = true;
            for (int i = 0; i < raceCarAIs.Length; i++)
            {
                raceCarAIs[i].enabled = true;
            }
            carMovement.enabled = true;
            Destroy(countdownTextObject);
        }
    }
}
