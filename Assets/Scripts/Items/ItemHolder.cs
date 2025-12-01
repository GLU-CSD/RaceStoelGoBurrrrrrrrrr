using UnityEngine;
using UnityEngine.InputSystem;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] InputActionReference useItem;
    [SerializeField] Transform itemHolder;
    public GameObject CurrentItem;
    public GameObject cloneCurrentItem;

    private void Update()
    {
        if (useItem.action.IsPressed())
        {
            Destroy(cloneCurrentItem);
        }
    }
    public void SpawnItem()
    {
        cloneCurrentItem = Instantiate(CurrentItem, itemHolder.transform);
    }
}
