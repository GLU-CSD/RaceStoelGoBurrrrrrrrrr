using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    [SerializeField] private CheckPoints[] checkPoints;
    public int lapCount = 1;

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
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
}
