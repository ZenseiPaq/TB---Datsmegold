using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int totalEnemiesKilled;
    public Dictionary<string, int> enemyKillCounts = new Dictionary<string, int>
    {
        { "Warlock", 0 },
        { "Barbarian", 0 },
        { "Fighter", 0 },
        { "Archer", 0 }
    };
    public int playerRemainingHealth;
    public float timeTaken;
    public int turnsTaken;
    public int finalTurnNumber;
    public int finalCompletionTime;

    private TextMeshProUGUI totalEnemiesText;
    private TextMeshProUGUI warlockKillsText;
    private TextMeshProUGUI barbarianKillsText;
    private TextMeshProUGUI fighterKillsText;
    private TextMeshProUGUI archerKillsText;
    private TextMeshProUGUI playerHealthText;
    private TextMeshProUGUI timeTakenText;
    private TextMeshProUGUI turnsTakenText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        playerRemainingHealth = 3;
        timeTaken = 0;
        turnsTaken = 0;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "End")
        {
            SetupEndSceneUI();
            DisplayEndGameStats();
        }
    }

    private void SetupEndSceneUI()
    {
        totalEnemiesText = GameObject.Find("TotalEnemiesText").GetComponent<TextMeshProUGUI>();
        warlockKillsText = GameObject.Find("WarlockKillsText").GetComponent<TextMeshProUGUI>();
        barbarianKillsText = GameObject.Find("BarbarianKillsText").GetComponent<TextMeshProUGUI>();
        fighterKillsText = GameObject.Find("FighterKillsText").GetComponent<TextMeshProUGUI>();
        archerKillsText = GameObject.Find("ArcherKillsText").GetComponent<TextMeshProUGUI>();
        playerHealthText = GameObject.Find("PlayerHealthText").GetComponent<TextMeshProUGUI>();
        timeTakenText = GameObject.Find("TimeTakenText").GetComponent<TextMeshProUGUI>();
        turnsTakenText = GameObject.Find("TurnsTakenText").GetComponent<TextMeshProUGUI>();
    }

    public void RegisterEnemyDeath(string enemyType)
    {
        if (enemyKillCounts.ContainsKey(enemyType))
        {
            enemyKillCounts[enemyType]++;
            totalEnemiesKilled++;
            Debug.Log($"{enemyType} killed. Total enemies killed: {totalEnemiesKilled}.");
        }
        else
        {
            Debug.LogWarning($"Unknown enemy type: {enemyType}");
        }
    }

    private void DisplayEndGameStats()
    {
        if (PlayerController.Instance != null)
        {
            playerRemainingHealth = PlayerController.Instance.playerHealth;
        }

        Debug.Log($"Total Enemies Killed: {totalEnemiesKilled}");
        Debug.Log($"Warlocks Killed: {enemyKillCounts["Warlock"]}");
        Debug.Log($"Barbarians Killed: {enemyKillCounts["Barbarian"]}");
        Debug.Log($"Fighters Killed: {enemyKillCounts["Fighter"]}");
        Debug.Log($"Archers Killed: {enemyKillCounts["Archer"]}");
        Debug.Log($"Player Health Remaining: {playerRemainingHealth}");
        Debug.Log($"Time Taken: {finalCompletionTime}");
        Debug.Log($"Turns Taken: {finalTurnNumber}");
        
        totalEnemiesText.text = "Total Enemies Killed: " + totalEnemiesKilled;
        warlockKillsText.text = "Warlocks Killed: " + enemyKillCounts["Warlock"];
        barbarianKillsText.text = "Barbarians Killed: " + enemyKillCounts["Barbarian"];
        fighterKillsText.text = "Fighters Killed: " + enemyKillCounts["Fighter"];
        archerKillsText.text = "Archers Killed: " + enemyKillCounts["Archer"];
        playerHealthText.text = "Player Health Remaining: " + playerRemainingHealth;
        timeTakenText.text = "Time Taken: " + finalCompletionTime + " seconds";
        turnsTakenText.text = "Turns Taken: " + finalTurnNumber;
    }

    public void UpdatePlayerHealth(int health)
    {
        playerRemainingHealth = health;
        Debug.Log($"Player health updated to: {playerRemainingHealth}");
    }

    public void UpdateTimeTaken(float time)
    {
        timeTaken = time;
        Debug.Log($"Time taken updated to: {timeTaken}");
    }

    public void IncrementTurns()
    {
        turnsTaken++;
        Debug.Log($"Turns taken incremented to: {turnsTaken}");
    }
public void SaveFinalStats(int turns, float time)
{
    finalTurnNumber = turns;
    finalCompletionTime = Mathf.FloorToInt(time);
}

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}