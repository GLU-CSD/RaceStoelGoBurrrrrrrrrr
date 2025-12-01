using UnityEngine;

public class ItemBoxSpawner : MonoBehaviour
{
    [SerializeField] private GameObject itemBox;
    [SerializeField] private GameObject itemBoxClone;
    [SerializeField] private Transform spawnPosition;
    private float spawnCooldown = 1;
    private void Update()
    {
        if (itemBoxClone == null)
        {
            spawnCooldown -= Time.deltaTime;
            if (spawnCooldown < 0)
            {
                itemBoxClone = Instantiate(itemBox, spawnPosition.position, Quaternion.identity);
                spawnCooldown = 1;
            }
        }
    }
}
