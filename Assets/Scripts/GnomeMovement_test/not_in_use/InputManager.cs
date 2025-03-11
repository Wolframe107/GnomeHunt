using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    AnimatorManager animatorManager;
    private float moveAmount;
    public Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;

    private void Awake() {
        animatorManager = GetComponent<AnimatorManager>();
    }
    private void OnEnable() {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.GnomeMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        playerControls.Enable();
    }

    private void OnDisable() {
        playerControls.Disable();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
    }
}
