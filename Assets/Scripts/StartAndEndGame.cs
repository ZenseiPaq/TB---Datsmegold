using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class StartAndEndGame : MonoBehaviour
{
    public int turnNumber;
    public float startTime;
    public float completionTime;

    public TextMeshProUGUI turnText;
    public TextMeshProUGUI timerText;
    public GameObject victoryCanvas;

    public GameObject normalCanvas;
    public GameObject endScreenCanvas;
    public GameObject overlappingCanvas;
    public GameObject titleScreen;
    public GameObject optionsCanvas;
    public PlayerController playerController;
    public TextMeshProUGUI currentBattleText;
    public int currentBattle;
    void Start()
    {
        turnText = GameObject.Find("TurnText").GetComponent<TextMeshProUGUI>();
        timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        currentBattleText = GameObject.Find("BattleCount").GetComponent<TextMeshProUGUI>();
        normalCanvas = GameObject.Find("MainOverlay");
        endScreenCanvas = GameObject.Find("RetryScreen");
        overlappingCanvas = GameObject.Find("GameOver");
        victoryCanvas = GameObject.Find("VictoryScreen");
        titleScreen = GameObject.Find("TitleScreen");
        optionsCanvas = GameObject.Find("OptionsScreen");

        normalCanvas.SetActive(true);
        endScreenCanvas.SetActive(false);
        overlappingCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
        titleScreen.SetActive(false);
        optionsCanvas.SetActive(false);

        Scene currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == "Start")
        {
            titleScreen.SetActive(true);
        }


        startTime = Time.time;
        UpdateUI();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    void Update()
    {
        currentBattleText.text = ("Battle "+ currentBattle);
        completionTime = Time.time - startTime;
        UpdateUI();
    }

    public void AddTurn()
    {

        turnNumber += 1;
        UpdateUI();
    }
    public void ResetTurn()
    {
        turnNumber = 0;
        UpdateUI();
    }
    public void YouDied()
    {
        normalCanvas.SetActive(false);
        endScreenCanvas.SetActive(true);
        StartCoroutine(RetryScreen());
    }

    private IEnumerator RetryScreen()
    {
        yield return new WaitForSeconds(3f);
        overlappingCanvas.SetActive(true);
    }

    private void UpdateUI()
    {
        if (turnText != null && turnNumber != 0) turnText.text = "Turn: " + turnNumber;
        if (timerText != null) timerText.text = "Time: " + Mathf.Floor(completionTime).ToString() + "s";
    }
    public void ShowVictoryScreen()
    {
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
            overlappingCanvas.SetActive(false);
            normalCanvas.SetActive(true);
            endScreenCanvas.SetActive(false);

            turnText.text = "Turn: " + turnNumber;
            timerText.text = "Time: " + Mathf.Floor(completionTime).ToString() + "s";

            Time.timeScale = 0;
        
            Debug.Log("Victory screen displayed.");
            StartCoroutine(GoToEndGame());
        }
        else
        {
            Debug.LogWarning("Victory canvas is not assigned!");
        }
    }
    private IEnumerator GoToEndGame()
    {
        yield return new WaitForSecondsRealtime(3f);
        Debug.Log("Loading End Scene...");
        SceneManager.LoadScene("End");
    }

    public void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Game");
    }
    public void OnOptionsButtonClicked()
    {
        optionsCanvas.SetActive(true);
        titleScreen.SetActive(false);
    }
    public void OnReturnButtonClicked()
    {
        optionsCanvas.SetActive(false);
        titleScreen.SetActive(true);
    }
    public void MainMenuButtonClicked()
    {
        SceneManager.LoadScene("Start");
    }
    public void EscapeMainMenu()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if(currentScene.name == "Start")
        {
            return;
        }

        if (titleScreen.activeSelf)
        {
            optionsCanvas.SetActive(false);
            titleScreen.SetActive(false);
        }
        else
        {
            titleScreen.SetActive(true);
        }
    }

    public void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    public void BackToTitleScreen()
    {
        optionsCanvas.SetActive(false);
        titleScreen.SetActive(true);
    }
    public void ShowBattleNumber()
    {
        currentBattleText.text = ("Battle " + currentBattle);
        if(currentBattle >= 4)
        {
            currentBattleText.text = ("YOU'VE SAVED YOUR GOLD!");
        }
    }
}