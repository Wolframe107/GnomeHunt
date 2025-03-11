using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.Netcode;
using TMPro;
using UnityEngine.VFX;
using System.Collections;
using System.Collections.Generic;

public class GremlinTimer : NetworkBehaviour
{
    public float maxTime = 1.0f;
    public float regenerationSpeed = 0.5f;
    public float timer;
    private float timerFraction;
    public Rigidbody rb;
//    private Canvas lifebarCanvas;
    public float canvasActivationDelay = 0.9f; // Delay before activating the canvas
    public float velocityThreshold = 0.1f; // Adjustable threshold for "close to zero" velocity
    public VisualEffect smokePoof;

    private bool playParticles = false;
    private bool hasTransformed = false;
    public GameObject gnomePrefab;

    private Dictionary<ulong, GameObject> clientGameObjects;
    // Start is called before the first frame update
    void Start()
    {
        clientGameObjects = FindObjectOfType<RelayManager>().clientGameObjects;

        if (IsServer) {
            if (smokePoof != null)
            {
                smokePoof.Play();
            }
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            timer = 0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer) {
            
            Vector3 velocity = rb.velocity;
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

            if (timer >= (maxTime - 0.1f) && !hasTransformed) {
                hasTransformed = true;
                ulong clientId = gameObject.GetComponent<NetworkObject>().OwnerClientId;

                spawnGnome(clientId);
                StartCoroutine(DelayedDespawn(0.2f));              
            }
        }
    }

    void spawnGnome(ulong clientId) {
        
        // Instantiate the new gnome object at the current object's position and rotation
        GameObject obj = Instantiate(gnomePrefab, transform.position, transform.rotation);

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
        NetworkObject gremlinNetwork = gameObject.GetComponent<NetworkObject>();
        gremlinNetwork.Despawn();
    }
}
