using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class StartScreen : MonoBehaviour
{
    [SerializeField] private InputActionReference gasButton;
    private float gasValue;
    private void Update()
    {
        gasValue = gasButton.action.ReadValue<float>();
        gasValue = 1f - gasValue;

        if (gasValue >= 0.1)
        {
            SceneManager.LoadScene(1);
        }
    }
}
