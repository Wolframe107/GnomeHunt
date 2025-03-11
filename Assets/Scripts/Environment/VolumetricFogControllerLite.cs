using UnityEngine;
using MirzaBeig.VolumetricFogLite;

[ExecuteInEditMode]
public class VolumetricFogController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light mainDirectionalLight;
    [SerializeField] private VolumetricFogRendererFeatureLite fogRendererFeature;
    private Material fogMaterial;

    [Header("Fog Settings")]
    [Range(1, 128)]
    [SerializeField] private int raymarchSteps = 35;
    [Range(0f, 1f)]
    [SerializeField] private float raymarchNoise = 0.04f;
    [Range(1, 8)]
    [SerializeField] private int downsampleLevel = 4;
    [Range(0f, 1f)]
    [SerializeField] private float fogDensity = 0.9f;

    // Cache property IDs for better performance
    private static readonly int RaymarchStepsID = Shader.PropertyToID("_RaymarchSteps");
    private static readonly int RaymarchNoiseID = Shader.PropertyToID("_RaymarchNoise");
    private static readonly int DensityID = Shader.PropertyToID("_Density");

    private void OnEnable()
    {
        InitializeReferences();
    }

    private void InitializeReferences()
    {
        // Try to find the directional light if not assigned
        if (mainDirectionalLight == null)
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach (Light light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    mainDirectionalLight = light;
                    break;
                }
            }
        }

        // Get the fog material from the renderer feature
        if (fogRendererFeature != null && fogMaterial == null)
        {
            fogMaterial = fogRendererFeature.settings.fogMaterial;
        }
    }

    private void Update()
    {
        if (fogMaterial == null || fogRendererFeature == null)
        {
            Debug.LogWarning("Fog material or renderer feature not assigned!");
            return;
        }

        // Update material properties
        fogMaterial.SetInt(RaymarchStepsID, raymarchSteps);
        fogMaterial.SetFloat(RaymarchNoiseID, raymarchNoise);
        fogMaterial.SetFloat(DensityID, fogDensity);

        // Update renderer feature properties
        fogRendererFeature.SetDownsampleLevel(downsampleLevel);

        // Update light reference in the material if available
        if (mainDirectionalLight != null)
        {
            // Assuming your shader uses these properties
            Shader.SetGlobalVector("_MainLightPosition", mainDirectionalLight.transform.forward);
            Shader.SetGlobalVector("_MainLightColor", mainDirectionalLight.color * mainDirectionalLight.intensity);
        }
    }

    // Public methods for external control
    public void SetRaymarchSteps(int steps)
    {
        raymarchSteps = Mathf.Clamp(steps, 1, 128);
    }

    public void SetRaymarchNoise(float noise)
    {
        raymarchNoise = Mathf.Clamp01(noise);
    }

    public void SetDownsampleLevel(int level)
    {
        downsampleLevel = Mathf.Clamp(level, 1, 8);
    }

    public void SetFogDensity(float density)
    {
        fogDensity = Mathf.Clamp01(density);
    }

    public void SetMainLight(Light light)
    {
        if (light.type == LightType.Directional)
        {
            mainDirectionalLight = light;
        }
        else
        {
            Debug.LogWarning("Only directional lights can be set as the main light source.");
        }
    }
}