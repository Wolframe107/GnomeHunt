using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class InputManagerNetwork : NetworkBehaviour
{
    private PlayerControls playerControls;
    private PlayerManagerNetwork managerNetwork;
    private PlayerLocomotionNetwork locomotionNetwork;    
    private Vector2 movementInput;
    private Vector2 lastMovementInput = Vector2.zero;
    private Dictionary<ulong, GameObject> clientGameObjects;

    private void Awake() {
        managerNetwork = GetComponent<PlayerManagerNetwork>();
        locomotionNetwork = GetComponent<PlayerLocomotionNetwork>();
        clientGameObjects = FindObjectOfType<RelayManager>().clientGameObjects;
        Debug.Log($"IsClient: {IsClient}, IsServer: {IsServer}, IsOwner: {IsOwner}");
        if (clientGameObjects == null)
        {
            Debug.LogError("Dictionary not found in the scene");
        }
        else{
            Debug.LogError("Dictionary was found");
        }
    }

    private void Start()
    {
        if (!IsClient)
        {
            Debug.LogError("This is not a client. Exiting input setup.");
            return;
        }

        InitializePlayerControls();
    }

    private void InitializePlayerControls()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.GnomeMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
        }

        Debug.Log("PlayerControls initialized on client.");
        playerControls.Enable();
    }

    public void ReadAndSendInput()
    {
        // Client sends input to server even if it is not the owner of the object
        // Only send input when it has changed or after a set interval has passed
        if(playerControls == null){
            Debug.Log("No player controls!!!");
        }

        if (movementInput != lastMovementInput)
        {
            Debug.Log("Sending movement input to server: " + movementInput);
            
            // Send the input to the server
            SendMovementInputToServerRpc(movementInput);
            
            // Update the last movement input to the current input
            lastMovementInput = movementInput;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMovementInputToServerRpc(Vector2 input, ServerRpcParams rpcParams = default)
    {   
        ulong clientId = rpcParams.Receive.SenderClientId;
        Debug.Log("Received input on server: " + input + " from client " + clientId);

        // Assuming relayManager is already set up as a reference to the RelayManager GameObject
        if (clientGameObjects.TryGetValue(clientId, out GameObject clientObject))
        {
            PlayerLocomotionNetwork clientLocomotion = clientObject.GetComponent<PlayerLocomotionNetwork>();
            if (clientLocomotion != null)
            {
                clientLocomotion.SetMovementInput(input);
            }
            else
            {
                Debug.LogError($"PlayerLocomotionNetwork component not found on client object with ID {clientId}");
            }
        }
        else
        {
            Debug.LogError($"Client with ID {clientId} not found in RelayManager's dictionary.");
        }
    }


    private void Update() {
        // Handle input every frame on the client
        if (!IsServer)  // Clients should still handle input
        {
            ReadAndSendInput();
        }
    }
}
