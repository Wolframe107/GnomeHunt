using UnityEngine;
using TMPro;  // Make sure you have the TextMeshPro namespace if you're using TMP

public class UppercaseText : MonoBehaviour
{
    // Reference to the TMP_InputField (if using TextMeshPro) or InputField (if using standard UI)
    public TMP_InputField tmpInputField;

    void Start()
    {
        if (tmpInputField != null)
        {
            // Add a listener to detect text changes
            tmpInputField.onValueChanged.AddListener(ForceUppercase);
        }
    }

    // This method is called every time the input field's value changes
    void ForceUppercase(string input)
    {
        // Convert text to uppercase
        tmpInputField.text = input.ToUpper();
    }

    void OnDestroy()
    {
        // Remove the listener when the object is destroyed to avoid memory leaks
        if (tmpInputField != null)
        {
            tmpInputField.onValueChanged.RemoveListener(ForceUppercase);
        }
    }
}
