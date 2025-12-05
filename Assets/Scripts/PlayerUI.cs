using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private InputActionReference toggle;
    [SerializeField] private GameObject playerCanvas;

    private void Update()
    {
        if (toggle.action.IsPressed())
        {
            playerCanvas.SetActive(true);
        }
        else
        {
            playerCanvas.SetActive(false);
        }

    }
}
