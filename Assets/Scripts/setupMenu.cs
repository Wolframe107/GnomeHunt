using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class setupMenu : MonoBehaviour
{
    [SerializeField] private GameObject XR;
    [SerializeField] private GameObject menuCamera;
    [SerializeField] private Button hostBtn;
    [SerializeField] private Button joinBtn;

/*    private void Awake() {
        hostBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartServer();
            menuCamera.SetActive(false);
            XR.SetActive(true);
        });

        joinBtn.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            menuCamera.SetActive(false);
        });

    }  */


    public void hostGame(){

        NetworkManager.Singleton.StartServer();
        menuCamera.SetActive(false);
        XR.SetActive(true);
    }

    public void joinGame(){
        NetworkManager.Singleton.StartClient();
        menuCamera.SetActive(false);
    } 
}
