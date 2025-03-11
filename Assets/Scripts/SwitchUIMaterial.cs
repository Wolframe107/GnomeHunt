using UnityEngine;
using UnityEngine.UI;

public class SwitchUIMaterial : MonoBehaviour
{
    public Material myMaterial; // Assign this in the inspector or dynamically

    void Start()
    {
        // Get all UI elements in the canvas
        Canvas canvas = GetComponent<Canvas>();
        if (canvas != null)
        {
            // Loop through all child Graphic components (e.g., Image, Text, etc.)
            Graphic[] uiElements = canvas.GetComponentsInChildren<Graphic>();
            foreach (Graphic uiElement in uiElements)
            {
                // Set the custom material for each UI element
                uiElement.material = myMaterial;
            }
        }
    }
}
