using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP; // UnityTransport
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models; // For Allocation and RelayServerData
using TMPro;

public class mobile_RelayManager : MonoBehaviour
{
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject codeInput;
    [SerializeField] private GameObject nameInput;
    [SerializeField] private GameObject codeText;
    

    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed In " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void JoinRelay () {
        bool joinSuccess = false; // Flag to track success

        try {
            string code = codeInput.GetComponent<TMP_InputField>().text;
            Debug.Log(code.ToUpper());
            if (string.IsNullOrEmpty(code)) return;

            Debug.Log("Tried to join " + code.ToUpper());

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(code.ToUpper());

            RelayServerData relayServerData = new RelayServerData(joinAllocation, "wss");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            NetworkManager.Singleton.StartClient();
            joinSuccess = true; // Set flag to true if no exceptions occur

            // Assign player name
            string playerName = nameInput.GetComponent<TMP_InputField>().text;
            if (string.IsNullOrEmpty(playerName)) {
                GameObject.Find("NameStorage").GetComponent<nameStorage>().setName("Unnamed gnome");
            } else {
                GameObject.Find("NameStorage").GetComponent<nameStorage>().setName(playerName);
            }

        } catch (RelayServiceException e) {
            Debug.LogError("Failed to join relay: " + e.Message);
        }

        // Only deactivate menu elements if join was successful
        if (joinSuccess) {
            menuCamera.SetActive(false);
            menuCanvas.SetActive(false);
        }
    }
}
