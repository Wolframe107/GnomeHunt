using UnityEngine;
using System.Collections;

public class MusicManager : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 2f; // Duration of fade-in/out in seconds
    public float maxVolume = 0.2f;  // Maximum volume level

    [HideInInspector]
    public bool IsFadeComplete = false;

    void Start()
    {
        // Ensure the AudioSource is assigned
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Start with volume at 0
        audioSource.volume = 0f;

        // Start playing the music
        audioSource.Play();

        // Start fading in
        StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        IsFadeComplete = false;
        float currentTime = 0f;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, maxVolume, currentTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = maxVolume;
        IsFadeComplete = true;
    }

    public IEnumerator FadeOut()
    {
        IsFadeComplete = false;
        float currentTime = 0f;
        float startVolume = audioSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, currentTime / fadeDuration);
            yield return null;
        }

        audioSource.volume = 0f;
        audioSource.Stop();
        IsFadeComplete = true;
    }
}