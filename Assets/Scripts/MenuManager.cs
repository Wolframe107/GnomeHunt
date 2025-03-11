using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management
using System.Collections;          // Required for IEnumerator

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public CanvasGroup instructionPanel1CanvasGroup;
    public CanvasGroup instructionPanel2CanvasGroup;
    public CanvasGroup blackOverlayCanvasGroup;
    public float instructionDisplayTime = 5f; // Time in seconds
    public float fadeDuration = 2f;           // Duration for fade in/out

    public MusicManager musicManager;

    public void OnPlayButtonClicked()
    {
        // Hide main menu UI
        mainMenuPanel.SetActive(false);

        // Start fading in black overlay
        StartCoroutine(FadeCanvasGroup(blackOverlayCanvasGroup, 0f, 1f, fadeDuration));

        // Start the instruction sequence
        StartCoroutine(InstructionSequence());
    }

    private IEnumerator InstructionSequence()
    {
        // Wait for black overlay to finish fading in
        yield return new WaitForSeconds(fadeDuration);

        // Fade in first instruction panel
        instructionPanel1CanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(instructionPanel1CanvasGroup, 0f, 1f, fadeDuration));

        // Wait for the instruction display time
        yield return new WaitForSeconds(instructionDisplayTime);

        // Fade out first instruction panel
        yield return StartCoroutine(FadeCanvasGroup(instructionPanel1CanvasGroup, 1f, 0f, fadeDuration));
        instructionPanel1CanvasGroup.gameObject.SetActive(false);

        // Fade in second instruction panel
        instructionPanel2CanvasGroup.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(instructionPanel2CanvasGroup, 0f, 1f, fadeDuration));

        // Wait for the instruction display time
        yield return new WaitForSeconds(instructionDisplayTime);

        // Fade out second instruction panel
        yield return StartCoroutine(FadeCanvasGroup(instructionPanel2CanvasGroup, 1f, 0f, fadeDuration));
        instructionPanel2CanvasGroup.gameObject.SetActive(false);

        // Start fading out the menu music after instructions
        if (musicManager != null)
        {
            StartCoroutine(musicManager.FadeOut());
        }

        // Wait until the music has finished fading out
        if (musicManager != null)
        {
            yield return new WaitUntil(() => musicManager.IsFadeComplete);
        }

        // Start the game
        StartGame();
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup canvasGroup, float start, float end, float duration)
    {
        float counter = 0f;
        canvasGroup.alpha = start;
        while (counter < duration)
        {
            counter += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, counter / duration);
            yield return null;
        }
        canvasGroup.alpha = end;
    }

    private void StartGame()
    {
        SceneManager.LoadScene("GnomeWorldMultiplayer");
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}