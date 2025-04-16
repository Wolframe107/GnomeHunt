/* using UnityEngine;
using Unity.XR.CoreUtils;

public class VRBodyTracker : MonoBehaviour
{
    [SerializeField] private XROrigin xrOrigin;

    private void Start()
    {
        if (xrOrigin == null)
            xrOrigin = FindObjectOfType<XROrigin>();
    }

    private void Update()
    {
        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            transform.position = xrOrigin.Camera.transform.position;
            transform.rotation = Quaternion.Euler(0, xrOrigin.Camera.transform.eulerAngles.y, 0);
        }
    }
} */