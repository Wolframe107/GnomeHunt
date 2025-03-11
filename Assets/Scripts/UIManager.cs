using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private float startSeconds = 180;

    [Header("Game Timer UI")]
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject timerBackground;           // New background reference
    [SerializeField] private GameObject timerTextVR;
    [SerializeField] private GameObject timerTextVRBackground;
    [SerializeField] private GameObject gnomeText;
    [SerializeField] private GameObject gnomeBackground;           // New background reference
    [SerializeField] private GameObject gnomeTextVR;
    [SerializeField] private GameObject gnomeTextVRBackground;
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject winnerText;

    [Header("VR End Screen")]
    [SerializeField] private GameObject endScreenVR;
    [SerializeField] private GameObject winnerTextVR;
    [SerializeField] private GameObject gnomeWinIconVR;
    [SerializeField] private GameObject hunterWinIconVR;

    [Header("End Screen Icons")]
    [SerializeField] private GameObject gnomeWinIcon;
    [SerializeField] private GameObject hunterWinIcon;
    
    [Header("Countdown UI - Hunter")]
    [SerializeField] private GameObject hunterCountdownText;
    [SerializeField] private GameObject gameStartCountdownMessage;


    [Header("Countdown UI - Gnomes")]
    [SerializeField] private GameObject gnomeCountdownText;
    [SerializeField] private GameObject gnomeCountdownBackground;   // New background reference
    [SerializeField] private GameObject gameStartMessageText;
    [SerializeField] private GameObject gameStartMessageBackground;

    [Header("Player Settings")]
    [SerializeField] private GameObject playerToMove;
    [SerializeField] private Vector3 gameStartPosition = new Vector3(0f, -0.5f, -5f);

    public AudioSource huntersWon;
    public AudioSource gnomesWon;
    public bool soundStartPlaying = false;

    private float timer;
    private int timerSeconds;
    private int timerMinutes;
    private bool timerRunning = false;
    private int gnomesLeft;
    private bool gameStarted = false;

    [SerializeField] private float countdownDuration = 5f;
    [SerializeField] private float gameStartMessageDuration = 0.8f;

    private float countdownTimer;

    void Start()
    {
        // Initialize variables
        timer = startSeconds;
        timerSeconds = (int)startSeconds % 60;
        timerMinutes = (int)timer / 60;
        gnomesLeft = NetworkManager.Singleton.ConnectedClients.Count;

        // Hide all game UI elements initially
        SetGameUIVisibility(false);

        // Activate the GameStartCountdownMessage at the start
        if (gameStartCountdownMessage != null)
            gameStartCountdownMessage.SetActive(true);

        // Activate gnomeCountdownText and its background at the start
        if (gnomeCountdownText != null) gnomeCountdownText.SetActive(true);
        if (gnomeCountdownBackground != null) gnomeCountdownBackground.SetActive(true);

        // Activate hunterCountdownText at the start
        if (hunterCountdownText != null) hunterCountdownText.SetActive(true);

        // Hide the GO! text and its background initially
        if (gameStartMessageText != null) gameStartMessageText.SetActive(false);
        if (gameStartMessageBackground != null) gameStartMessageBackground.SetActive(false);

        // Hide the end screen icons and VR end screen initially
        if (gnomeWinIcon != null) gnomeWinIcon.SetActive(false);
        if (hunterWinIcon != null) hunterWinIcon.SetActive(false);
        if (endScreenVR != null) endScreenVR.SetActive(false);
        if (gnomeWinIconVR != null) gnomeWinIconVR.SetActive(false);
        if (hunterWinIconVR != null) hunterWinIconVR.SetActive(false);

        // Initialize countdown
        countdownTimer = countdownDuration;

        // Start the countdown coroutine
        StartCoroutine(StartCountdown());
    }

    void Update()
    {
        // Do nothing if the game hasn't started yet
        if (!gameStarted)
            return;

        updateGnomeText();
        if (timerRunning && timer > 0)
        {
            updateTimer();
            updateTimerUI();
        }

        if (timer <= 0 || gnomesLeft == 0)
        {
            EndGame();
        }
    }

    IEnumerator StartCountdown()
    {
        TextMeshProUGUI hunterTMP = hunterCountdownText?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI gnomeTMP = gnomeCountdownText?.GetComponent<TextMeshProUGUI>();

        while (countdownTimer > 0)
        {
            string countdownNumber = Mathf.Ceil(countdownTimer).ToString();

            // Update hunter countdown
            if (hunterTMP != null) hunterTMP.text = countdownNumber;

            // Update gnome countdown with prefix
            if (gnomeTMP != null) gnomeTMP.text = "Hunter Arrives In: " + countdownNumber;

            yield return new WaitForSeconds(1f);
            countdownTimer--;
        }

        // Deactivate the GameStartCountdownMessage before showing "GO!" message
        if (gameStartCountdownMessage != null)
            gameStartCountdownMessage.SetActive(false);

        // Deactivate gnomeCountdownText and its background
        if (gnomeCountdownText != null) gnomeCountdownText.SetActive(false);
        if (gnomeCountdownBackground != null) gnomeCountdownBackground.SetActive(false);

        // Show "GO!" message and its background
        if (hunterTMP != null) hunterTMP.text = "GO!";
        if (gameStartMessageBackground != null) gameStartMessageBackground.SetActive(true);

        if (gameStartMessageText != null)
        {
            gameStartMessageText.SetActive(true);
            gameStartMessageText.GetComponent<TextMeshProUGUI>().text = "GO!";
        }

        // Wait for GO! message duration
        yield return new WaitForSeconds(gameStartMessageDuration);

        // Hide all countdown-related UI
        if (hunterCountdownText != null) hunterCountdownText.SetActive(false);
        if (gameStartMessageText != null) gameStartMessageText.SetActive(false);
        if (gameStartMessageBackground != null) gameStartMessageBackground.SetActive(false);

        StartGame();
    }

    void StartGame()
    {
        gameStarted = true;
        timerRunning = true;

        // Show the game UI elements
        SetGameUIVisibility(true);

        // Move player to start position when the game starts
        MovePlayerToStartPosition();
    }

    void MovePlayerToStartPosition()
    {
        // Move the assigned player GameObject to the start position
        if (playerToMove != null)
        {
            playerToMove.transform.position = gameStartPosition;
        }
        else
        {
            Debug.LogWarning("Player to move is not assigned in the inspector.");
        }
    }

    void updateTimer()
    {
        timer -= Time.deltaTime;
        timerSeconds = (int)timer % 60;
        timerMinutes = (int)timer / 60;
    }

    void updateTimerUI()
    {
        string formattedTime = string.Format("{0:00}:{1:00}", timerMinutes, timerSeconds);
        if (timerText != null)
            timerText.GetComponent<TextMeshProUGUI>().text = formattedTime;
        if (timerTextVR != null)
            timerTextVR.GetComponent<TextMeshProUGUI>().text = formattedTime;
    }

    void updateGnomeText()
    {
        string gnomesLeftText = "Gnomes left: " + gnomesLeft.ToString();
        if (gnomeText != null)
            gnomeText.GetComponent<TextMeshProUGUI>().text = gnomesLeftText;
        if (gnomeTextVR != null)
            gnomeTextVR.GetComponent<TextMeshProUGUI>().text = gnomesLeftText;
    }

    public void startTimer()
    {
        timerRunning = true;
    }

    public void removeGnome()
    {
        gnomesLeft -= 1;
    }

    void EndGame()
    {
        // Show the end screen
        endScreen.SetActive(true);

        // Show the VR end screen
        if (endScreenVR != null)
            endScreenVR.SetActive(true);

        // Determine who won and update the UI accordingly
        if (gnomesLeft == 0)
        {
            // Hunter wins
            if (winnerText != null)
                winnerText.GetComponent<TextMeshProUGUI>().text = "The hunter wins";
            if (winnerTextVR != null)
                winnerTextVR.GetComponent<TextMeshProUGUI>().text = "The hunter wins";

            // Show Hunter win icons
            if (hunterWinIcon != null)
                hunterWinIcon.SetActive(true);
            if (hunterWinIconVR != null)
                hunterWinIconVR.SetActive(true);

            if (!soundStartPlaying)
            {
                huntersWon.Play();
                soundStartPlaying = true;
            }

        }
        else if (timer <= 0)
        {
            // Gnomes win
            if (winnerText != null)
                winnerText.GetComponent<TextMeshProUGUI>().text = "The gnomes win";
            if (winnerTextVR != null)
                winnerTextVR.GetComponent<TextMeshProUGUI>().text = "The gnomes win";

            // Show Gnome win icons
            if (gnomeWinIcon != null)
                gnomeWinIcon.SetActive(true);
            if (gnomeWinIconVR != null)
                gnomeWinIconVR.SetActive(true);

            if (!soundStartPlaying)
            {
                gnomesWon.Play();
                soundStartPlaying = true;
            }

        }

        StartCoroutine(LoadMainMenu(10));
    }

    IEnumerator LoadMainMenu(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(0);
    }

    void SetGameUIVisibility(bool visible)
    {
        // Show or hide standard UI elements and their backgrounds
        if (timerText != null) timerText.SetActive(visible);
        if (timerBackground != null) timerBackground.SetActive(visible);

        if (gnomeText != null) gnomeText.SetActive(visible);
        if (gnomeBackground != null) gnomeBackground.SetActive(visible);

        // Show or hide VR UI elements and their backgrounds
        if (timerTextVR != null) timerTextVR.SetActive(visible);
        if (timerTextVRBackground != null) timerTextVRBackground.SetActive(visible);

        if (gnomeTextVR != null) gnomeTextVR.SetActive(visible);
        if (gnomeTextVRBackground != null) gnomeTextVRBackground.SetActive(visible);
    }
}
