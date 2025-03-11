using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;

public class VRRenderLayerAssigner : MonoBehaviour
{
    [SerializeField]
    private int vrRenderLayerIndex = 1;

    [Header("Debug")]
    [SerializeField]
    private bool logAssignments = true;

    private uint vrRenderLayerMask;
    private HashSet<Renderer> processedRenderers = new HashSet<Renderer>();

    void Awake()
    {
        vrRenderLayerMask = 1u << vrRenderLayerIndex;
        AssignLayersToAllRenderers();
    }

    void OnEnable()
    {
        // Start checking for new children
        StartCoroutine(CheckForNewRenderers());
    }

    System.Collections.IEnumerator CheckForNewRenderers()
    {
        while (enabled)
        {
            CheckNewRenderers();
            yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
        }
    }

    void CheckNewRenderers()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        
        foreach (var renderer in renderers)
        {
            if (!processedRenderers.Contains(renderer))
            {
                AssignLayerToRenderer(renderer);
                processedRenderers.Add(renderer);
            }
        }
    }

    void AssignLayersToAllRenderers()
    {
        var renderers = GetComponentsInChildren<Renderer>(true);
        processedRenderers.Clear();

        foreach (var renderer in renderers)
        {
            AssignLayerToRenderer(renderer);
            processedRenderers.Add(renderer);
        }
    }

    void AssignLayerToRenderer(Renderer renderer)
    {
        if (renderer != null)
        {
            renderer.renderingLayerMask = vrRenderLayerMask;
            
            if (logAssignments)
            {
                Debug.Log($"[VRRenderLayerAssigner] Assigned layer {vrRenderLayerIndex} to: {renderer.gameObject.name}", renderer.gameObject);
            }
        }
    }

    // Public method to force refresh all assignments
    public void RefreshAllAssignments()
    {
        AssignLayersToAllRenderers();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        vrRenderLayerIndex = Mathf.Clamp(vrRenderLayerIndex, 0, 7);
    }
#endif
}