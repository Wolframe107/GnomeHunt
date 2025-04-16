using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Netcode;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GnomeTimer : NetworkBehaviour
{
    public float maxTime = 5f;
    public float regenerationSpeed = 0.5f;
    public float timer;
    public RectTransform timerBar;
    public float maxWidth = 90f;
    private float timerFraction;
    public Rigidbody rb;
    private Canvas lifebarCanvas;
    public float canvasActivationDelay = 0.9f; // Delay before activating the canvas
    public float velocityThreshold = 0.1f; // Adjustable threshold for "close to zero" velocity
    private bool playParticles = false;
    private bool hasTransformed = false;
    public GameObject gremlinPrefab;
    
    private Dictionary<ulong, GameObject> clientGameObjects;

    void Start()
    {

        clientGameObjects = FindObjectOfType<RelayManager>().clientGameObjects;

        if (IsServer) {
            
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            timer = maxTime;

            // Get the Lifebar Canvas component
            lifebarCanvas = transform.Find("Lifebar")?.GetComponent<Canvas>();
            if (lifebarCanvas == null)
            {
                Debug.LogError("Lifebar Canvas not found as a child of this GameObject!");
            }
            
            else
            {
                // Start the coroutine to activate the canvas after a delay
                StartCoroutine(ForceLifebarUpdate());
            }
        
        }
    }

    private IEnumerator ForceLifebarUpdate()
    {
        yield return null; // Wait for the next frame
        GameObject lifebar = transform.Find("Lifebar")?.gameObject;
        if (lifebar != null)
        {
            lifebar.SetActive(true);
            Debug.Log($"Forced Lifebar update. Active: {lifebar.activeSelf}");
           
            // If the Lifebar contains a Canvas, we can still force a rebuild of its layout
            Canvas lifebarCanvas = lifebar.GetComponent<Canvas>();
            if (lifebarCanvas != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(lifebarCanvas.GetComponent<RectTransform>());
            }
        }
        else
        {
            Debug.Log("Lifebar GameObject not found!");
        }
    }

    void Update()
    {
        if (IsServer) {
            
            Vector3 velocity = rb.linearVelocity;
            //Debug.Log($"Velocity: {velocity}, Magnitude: {velocity.magnitude}");

            if (velocity.magnitude <= velocityThreshold)
            {   
                //Debug.Log("Below threshold, decreasing timer");
                timer -= Time.deltaTime / 4f;
            }
            else
            {
                //Debug.Log("Above threshold, increasing timer");
                timer += Time.deltaTime * regenerationSpeed;
            }

            timer = Mathf.Clamp(timer, 0f, maxTime);
            timerFraction = timer / maxTime;

            // Update the width of the timer bar
            Vector2 sizeDelta = timerBar.sizeDelta;
            sizeDelta.x = timerFraction * maxWidth;
            timerBar.sizeDelta = sizeDelta; 

            if (timer <= 0.1f && !hasTransformed) {
                hasTransformed = true;
                ulong clientId = gameObject.GetComponent<NetworkObject>().OwnerClientId;

                spawnGremlin(clientId);
                StartCoroutine(DelayedDespawn(0.2f));              
            }
        }
    }

    void spawnGremlin(ulong clientId) {

        // Instantiate the new gnome object at the current object's position and rotation
        GameObject obj = Instantiate(gremlinPrefab, transform.position, transform.rotation);

        // Copy the player name if found
        Transform playerNameObj = transform.Find("PlayerHUD/PlayerName");
        if (playerNameObj != null)
        {
            TextMeshProUGUI playerNameText = playerNameObj.GetComponent<TextMeshProUGUI>();
            if (playerNameText != null)
            {
                string playerName = playerNameText.text;
                Transform OtherPlayerNameObj = obj.transform.Find("PlayerHUD/PlayerName");
                if (OtherPlayerNameObj != null)
                {
                    TextMeshProUGUI OtherPlayerNameText = OtherPlayerNameObj.GetComponent<TextMeshProUGUI>();
                    if (OtherPlayerNameText != null)
                    {
                        OtherPlayerNameText.text = playerName;
                    }
                }
            }
        }

        // Get the NetworkObject component and spawn the new gnome with ownership
        NetworkObject network = obj.GetComponent<NetworkObject>();
        network.SpawnWithOwnership(clientId);
    
        clientGameObjects[clientId] = obj;
    }

    IEnumerator DelayedDespawn(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Despawn the existing network object
        NetworkObject gnomeNetwork = gameObject.GetComponent<NetworkObject>();
        gnomeNetwork.Despawn();
    }
}