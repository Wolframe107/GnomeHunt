using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CameraPostProcessController : MonoBehaviour
{
    [System.Serializable]
    public class CameraSetup
    {
        public string label;
        public Camera camera;
        public UniversalRendererData rendererData;
        public Volume volume;  // Changed: Reference the Volume component instead of just the profile
        [Range(-5f, 5f)] public float exposure = 0f;
        [Range(0f, 1f)] public float vignetteIntensity = 0.5f;

        // Runtime profile that we can modify
        [HideInInspector]
        public VolumeProfile runtimeProfile;
    }

    [Header("Camera Setups")]
    [SerializeField] private CameraSetup vrCamera;
    [SerializeField] private CameraSetup overviewCamera;

    [Header("Shared Volume")]
    [SerializeField] private Volume globalVolume;

    private void Awake()
    {
        // Create runtime profiles
        InitializeCameraSetup(vrCamera);
        InitializeCameraSetup(overviewCamera);
    }

    private void InitializeCameraSetup(CameraSetup setup)
    {
        if (setup.volume == null || setup.volume.profile == null) return;

        // Create a runtime copy of the profile
        setup.runtimeProfile = ScriptableObject.Instantiate(setup.volume.profile);
        setup.volume.profile = setup.runtimeProfile;

        // Initialize values
        if (setup.runtimeProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.postExposure.value = setup.exposure;
        }

        if (setup.runtimeProfile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = setup.vignetteIntensity;
        }
    }

    private void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        UpdateCameraSetup(vrCamera);
        UpdateCameraSetup(overviewCamera);
    }

    private void UpdateCameraSetup(CameraSetup setup)
    {
        if (setup.runtimeProfile == null) return;

        // Update exposure
        if (setup.runtimeProfile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.postExposure.value = setup.exposure;
        }

        // Update vignette
        if (setup.runtimeProfile.TryGet(out Vignette vignette))
        {
            vignette.intensity.value = setup.vignetteIntensity;
        }
    }

    // Public methods for runtime control
    public void SetExposure(bool isVRCamera, float value)
    {
        var setup = isVRCamera ? vrCamera : overviewCamera;
        setup.exposure = value;
        UpdateCameraSetup(setup);
    }

    public void SetVignette(bool isVRCamera, float value)
    {
        var setup = isVRCamera ? vrCamera : overviewCamera;
        setup.vignetteIntensity = value;
        UpdateCameraSetup(setup);
    }
}