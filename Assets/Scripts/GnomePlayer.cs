using UnityEngine;
using UnityEngine.SceneManagement;

public class GnomePlayer : MonoBehaviour
{
    private bool isPlayer;
    private GnomeTimer gnomeTimer;

    void Start()
    {
        isPlayer = true;

        // Get the GnomeTimer component
        gnomeTimer = GetComponent<GnomeTimer>();
 
        if (gnomeTimer == null)
        {
            Debug.Log("GnomeTimer script not found on this GameObject!");
        }
        else if (SceneManager.GetActiveScene().name != "GnomeWorldMultiplayer")
        {
            // Deactivate the GnomeTimer script initially
            gnomeTimer.enabled = false;
        }

        //SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /*
    // This method is called every time a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (gnomeTimer != null)
        {
            // Activate the GnomeTimer script
            gnomeTimer.enabled = true;
            Debug.Log("GnomeTimer activated after scene change to: " + scene.name);
        }
    }
    */
    public void DestroyGnome()
    {
        Debug.Log("Gnome was destroyed...");

        GameObject controlCanvas = GameObject.Find("ControlCanvas");
        GameObject capturedCanvas = GameObject.Find("CapturedCanvas");

        if (controlCanvas != null && capturedCanvas != null)
        {
            // If ControlCanvas is found, disable it
            controlCanvas.SetActive(false);
            capturedCanvas.SetActive(true);
            Debug.Log("ControlCanvas found and disabled.");
        }
        
        Destroy(gameObject);
    }

    public bool CheckIfPlayer()
    {
        return isPlayer;
    }

    /*
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    */
}