using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;

public class RelayManager : MonoBehaviour
{
    private int maxPlayers = 6;
    [SerializeField] private TMP_Text code_ui;
    [SerializeField] private TMP_Text number_of_gnomes_ui;
    public Dictionary<ulong, GameObject> clientGameObjects = new Dictionary<ulong, GameObject>();
    private string allocationId;  // Store the allocation ID for shutdown

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

     private async void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        // Check if the server is already running
        if (NetworkManager.Singleton.IsServer)
        {
            Debug.Log("Server is already running.");
            ShutdownRelay();
        }

        // Initialize Unity services
        await UnityServices.InitializeAsync();

        // Check if the player is signed in
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Player not signed in. Signing in anonymously...");
            
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Signed In: " + AuthenticationService.Instance.PlayerId);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to sign in: " + e);
                return; // Stop further execution if sign-in fails
            }
        }
        else
        {
            Debug.Log("Player already signed in: " + AuthenticationService.Instance.PlayerId);
        }

        // Proceed with Relay allocation and starting the server
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Relay join code: " + joinCode);
            code_ui.text = joinCode;

            // Set up the Relay Server Data for UnityTransport
            RelayServerData relayServerData = new RelayServerData(allocation, "wss");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // Start the server
            NetworkManager.Singleton.StartServer();
            Debug.Log("Server started.");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("Relay service exception: " + e);
        }
    }

    // Shutdown function
    public void ShutdownRelay()
    {
        // Stop the network manager server
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }

        Debug.Log("Relay server shut down.");
    
    }

    private void OnClientConnected(ulong clientId)
    {
        int currentPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        Debug.Log("Client connected. Current player count: " + currentPlayers);
        number_of_gnomes_ui.text = currentPlayers + "/6";

        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var networkClient))
        {
            GameObject clientGameObject = networkClient.PlayerObject.gameObject;
            clientGameObjects[clientId] = clientGameObject;
            Debug.Log($"Client {clientId} connected with GameObject {clientGameObject.name}");
        }

        if (currentPlayers > maxPlayers)
        {
            Debug.Log("Max players reached. Kicking client with ID: " + clientId);
            NetworkManager.Singleton.DisconnectClient(clientId);
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {   
        clientGameObjects.Remove(clientId);
        number_of_gnomes_ui.text = (NetworkManager.Singleton.ConnectedClients.Count) + "/6";
        Debug.Log("Client disconnected. Current player count: " + (NetworkManager.Singleton.ConnectedClients.Count));
    }

    public GameObject GetClientGameObject(ulong clientId)
    {
        clientGameObjects.TryGetValue(clientId, out GameObject clientGameObject);
        return clientGameObject;
    }

    /*
    private void OnDestroy()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
    }
    */
}
