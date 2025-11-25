using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class StartScreen : MonoBehaviour
{
    [SerializeField] private InputActionReference gasButton;
    [SerializeField] private InputActionReference testOkeButton;
    private float gasValue = 0;
    private void Update()
    {
        gasValue = gasButton.action.ReadValue<float>();
        gasValue = 1f - gasValue;

        if (gasValue >= 1.1f)
        {
            SceneManager.LoadScene(1);
        }
        if (testOkeButton.action.IsPressed())
        {
            SceneManager.LoadScene(1);
        }
    }
}
