using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class start : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        // Load the game scene
        GameObject[] gnomePlayerObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (GameObject obj in gnomePlayerObjects)
        {
            if (obj.name == "GnomePlayerNetwork(Clone)")
            {
                Debug.Log("Found a gnome!");

                // Find the Lifebar canvas in children
                Canvas lifebarCanvas = obj.GetComponentInChildren<Canvas>(true);
                if (lifebarCanvas != null && lifebarCanvas.gameObject.name == "Lifebar")
                {
                    lifebarCanvas.enabled = true;
                    Debug.Log($"Activated Lifebar canvas for {obj.name}");
                }
                else
                {
                    Debug.LogWarning($"Lifebar canvas not found for {obj.name}");
                }
            }
        }
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
