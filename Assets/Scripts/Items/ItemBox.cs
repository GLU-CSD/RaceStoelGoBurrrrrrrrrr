using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    private ItemHolder itemHolder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            itemHolder = other.GetComponent<ItemHolder>();
            if (itemHolder.cloneCurrentItem == null)
            {
                itemHolder.CurrentItem = items[Random.Range(0, items.Length)];
                itemHolder.SpawnItem();
                Destroy(gameObject);
            }
        }
    }
}
