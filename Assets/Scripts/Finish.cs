using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private CheckPoints[] checkPoints;
    [SerializeField] private TextMeshProUGUI lapText;
    private int lapCount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (lapCount <= 3)
            {
                if (checkPoints[0].activated == true && checkPoints[1].activated == true && checkPoints[2].activated == true)
                {
                    lapCount++;
                    for (int i = 0; i < checkPoints.Length; i++)
                    {
                        checkPoints[i].activated = false;
                    }
                }
            }
        }
    }
    private void Update()
    {
        lapText.text = "Lap:" + " " + lapCount.ToString() + "/3";
        if (lapCount >= 4)
        {
            SceneManager.LoadScene(1);

        }
    }
}
