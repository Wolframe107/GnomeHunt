using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameRestart : MonoBehaviour
{
    private InputAction restartAction;

    private void OnEnable()
    {
        // Create and configure the input action
        restartAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/r", interactions: "press(behavior=1)");
        restartAction.AddBinding("<Keyboard>/leftCtrl").WithInteractions("press(behavior=1)");
        
        // Enable the action
        restartAction.Enable();

        // Register the action callback
        restartAction.performed += ctx => RestartGame();
    }

    private void OnDisable()
    {
        // Disable and unregister the action
        restartAction.Disable();
        restartAction.performed -= ctx => RestartGame();
    }

    private void RestartGame()
    {
        // Reload the first scene (index 0)
        SceneManager.LoadScene(0);
    }
}
