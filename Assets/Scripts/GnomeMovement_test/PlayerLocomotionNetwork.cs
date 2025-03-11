using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerLocomotionNetwork : NetworkBehaviour
{
    private Rigidbody playerRigidbody;
    private AnimatorManager animatorManager;

    private Vector3 moveDirection;
    float moveAmount;
    public float movementSpeed = 7.0f;
    public float rotationSpeed = 15.0f;
    private float verticalInput = 0.0f;
    private float horizontalInput = 0.0f;

    private void Awake() {
        playerRigidbody = GetComponent<Rigidbody>();
        animatorManager = GetComponent<AnimatorManager>();  // Reference to AnimatorManager
    }

    // Called by the server when receiving input from the client
    public void SetMovementInput(Vector2 movementInput)
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;
    }

    private void MoveGnome()
    {
        if (playerRigidbody.isKinematic) return;  // Prevent kinematic Rigidbody from setting velocity

        // Debug to check input values
        //Debug.Log("Horizontal Input: " + horizontalInput);
        //Debug.Log("Vertical Input: " + verticalInput);
        //Debug.Log("Move Direction: " + moveDirection);
        //Debug.Log("Movement Velocity: " + movementVelocity);

        // Calculate movement direction in world space
        moveDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount);

        // Apply movement to the Rigidbody (server-side)
        Vector3 movementVelocity = moveDirection * movementSpeed;
        
        // Debug to check if velocity is being set
        playerRigidbody.velocity = movementVelocity;
    }

    private void RotateGnome()
    {
        if (moveDirection == Vector3.zero) return;  // If no movement input, do not rotate

        // Calculate the target direction for rotation based on movement direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Apply the rotation
        transform.rotation = smoothedRotation;
    }

    private void FixedUpdate()
    {
        // Movement and rotation will be processed server-side during FixedUpdate
        if (IsServer)
        {
            MoveGnome();
            RotateGnome();
        }
    }
}
