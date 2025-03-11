using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerManagerNetwork : NetworkBehaviour
{
    InputManagerNetwork inputManager;
    Rigidbody playerRigidbody;
    private Dictionary<ulong, GameObject> clientGameObjects;
    public FixedString64Bytes PlayerName = "Unnamed gnome";
    public ulong playerId;
    public GameObject controlCanvas;
    public GameObject capturedCanvas;
    public GameObject lostConnectionCanvas;
    public VisualEffect smokePoof;

    private void Awake() {
        DontDestroyOnLoad(this.gameObject);
        inputManager = GetComponent<InputManagerNetwork>();
        playerRigidbody = GetComponent<Rigidbody>();    
        //smokePoof = GetComponent<ParticleSystem>();

        clientGameObjects = FindObjectOfType<RelayManager>().clientGameObjects;
    }
    
    public override void OnNetworkSpawn()
    {   
        Debug.Log($"OnNetworkSpawn - IsClient: {IsClient}, IsServer: {IsServer}, IsOwner: {IsOwner}");
        playerRigidbody.isKinematic = false;
        playerId = OwnerClientId;

        if (IsServer)
        {   
            NetworkObject.ChangeOwnership(NetworkManager.ServerClientId);

            // Move gnome to random location
            if(SceneManager.GetActiveScene().buildIndex == 0){
                float randomX = Random.Range(-6.0f, 2.2f);
                float randomZ = Random.Range(-7.0f, -4.5f);
                
                transform.position = new Vector3(randomX, transform.position.y, randomZ);
            }

            if (smokePoof != null)
            {
                smokePoof.Play();
            }
        }
        else
        {
            FixedString64Bytes username = GameObject.Find("NameStorage").GetComponent<nameStorage>().getName();
            SetUsernameServerRpc(username);
            playerRigidbody.isKinematic = true;
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if(!IsServer){
            Debug.Log("Lost Connection...");

            GameObject controlCanvas = GameObject.Find("ControlCanvas");
            GameObject lostConnectionCanvas = GameObject.Find("LostConnectionCanvas");

            if (controlCanvas != null && lostConnectionCanvas != null)
            {
                // If LostConnectionCanvas is found, disable it
                controlCanvas.SetActive(false);
                lostConnectionCanvas.SetActive(true);
                Debug.Log("LostConnectionCanvas found and disabled.");
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetUsernameServerRpc(FixedString64Bytes username, ServerRpcParams rpcParams = default) {
        ulong clientId = rpcParams.Receive.SenderClientId;

        clientGameObjects = FindObjectOfType<RelayManager>().clientGameObjects;
        
        Debug.Log("Received name on server: " + username);

        if (clientGameObjects.TryGetValue(clientId, out GameObject clientObject))
        {   
            if(clientObject != null){
                PlayerNameDisplay clientPlayerDisplay = clientObject.GetComponent<PlayerNameDisplay>();

                    if (clientPlayerDisplay != null)
                    {
                        clientPlayerDisplay.UpdatePlayerText(username);
                    }
                    else
                    {
                        Debug.LogError($"clientPlayerDisplay component not found on client object with ID {clientId}");
                    }
                }
            }
            
        else
        {
            Debug.LogError($"Client with ID {clientId} not found in RelayManager's dictionary.");
        }
        
    }
}
