using UnityEngine;
using UnityEngine.InputSystem;

public class ItemHolder : MonoBehaviour
{
    [SerializeField] InputActionReference useItem;
    [SerializeField] Transform itemHolder;
    public GameObject CurrentItem;
    public GameObject cloneCurrentItem;
    public GameObject itemLaunch;

    private void Update()
    {
        if (useItem.action.IsPressed())
        {
            if (cloneCurrentItem != null)
            {
                Destroy(cloneCurrentItem);
                Instantiate(CurrentItem, itemLaunch.transform.position, itemLaunch.transform.rotation);
            }

        }
    }
    public void SpawnItem()
    {
        cloneCurrentItem = Instantiate(CurrentItem, itemHolder.transform);
        cloneCurrentItem.GetComponent<Rigidbody>().isKinematic = true;
    }
}
