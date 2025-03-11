using UnityEngine;

public class TrackObject : MonoBehaviour
{
    public Transform target;   // The object to follow
    public Vector3 offset;     // Offset from the target

    private Camera mainCamera; // Reference to the camera

    void Start()
    {
        // Find the camera by name (make sure your camera has the correct name)
        mainCamera = GameObject.Find("OverheadCamera").GetComponent<Camera>();
    }

    void Update()
    {
        if (target != null && mainCamera != null)
        {
            // Convert the target's world position to screen position
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(target.position + offset);

            // Update the UI Image position
            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.position = screenPosition;

            // Immediately set rotation to zero to prevent any flicker
            rectTransform.localRotation = Quaternion.identity; // Using localRotation ensures no influence from parent transforms
        }
    }
}
