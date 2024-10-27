using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    public CanvasGroup bannerCanvasGroup;
    public TurnState state;
    public List<GameObject> players;
    public List<GameObject> enemies;
    private int currentEnemyIndex = 0;
    public BannerManager bannerManager;
    public MenuManager menuManager;
    public PlayerController playerController;
    public StartAndEndGame gameState;
    public StartAndEndGame startAndEndGame;
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public int battleCount = 0;
    private Coroutine currentCoroutine;

    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Idle,
        Endturn,
        Victory,
        Defeat,
        EnemiesDead
    }

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
        battleCount = 1;  
        if (gameState == null)
        {
            gameState = FindObjectOfType<StartAndEndGame>();
            if (gameState == null) return;
        }
        startAndEndGame.currentBattle = battleCount;
        SpawnEnemies();
        StartPlayerTurn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            EndPlayerTurn();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndEnemyTurn();
        }

        if (players.Count == 0)
        {
            state = TurnState.Defeat;
        }
        else if (enemies.Count == 0)
        {
            state = TurnState.EnemiesDead;
        }

        switch (state)
        {
            case TurnState.Victory:
                HandleVictory();
                break;

            case TurnState.Defeat:
                HandleDefeat();
                break;

            case TurnState.PlayerTurn:
                break;

            case TurnState.EnemyTurn:
                break;
        }
    }

    public void StartPlayerTurn()
    {
        Debug.Log("Starting Player's Turn.");
        bannerManager.ShowBanner("Your turn");
        playerController.isShieldActive = false;
        StartCoroutine(StartPlayerTurnWithDelay());
    }

    private IEnumerator StartPlayerTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);
        menuManager.ShowAbilityMenu();
        state = TurnState.PlayerTurn;
        playerController.CheckCurrentHP();
        Debug.Log("Player Turn Active.");
    }

    public void EndPlayerTurn()
    {
        menuManager.HideAbilityMenu();
        StartCoroutine(EndPlayerTurnWithDelay());
        if (currentEnemyIndex == 0)
        {
            bannerManager.ShowBanner("Their Turn");
        }
    }

    private IEnumerator EndPlayerTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);
        state = TurnState.EnemyTurn;
        StartEnemyTurn();
    }

    public void StartEnemyTurn()
    {
        if (state == TurnState.EnemyTurn && enemies.Count > 0)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject currentEnemy = enemies[i];
            EnemyBehavior enemyController = currentEnemy.GetComponent<EnemyBehavior>();
            if (enemyController != null)
            {
                bool useHeal = enemyController.currentHealth < 40;
                if (useHeal)
                {
                    enemyController.HealSelf();
                    Debug.Log($"{enemyController.name} heals.");
                }
                else
                {
                    bool useRangedAttack = Random.Range(0f, 1f) > 0.5f;
                    if (useRangedAttack)
                    {
                        enemyController.PerformRangedAttack();
                        Debug.Log($"{enemyController.name} shoots.");
                    }
                    else
                    {
                        enemyController.PerformMeleeAttack();
                        Debug.Log($"{enemyController.name} stabs.");
                    }
                }
            }
            yield return new WaitForSeconds(2f);
        }
        EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        if (gameState == null)
        {
            Debug.LogError("GameState is null in EndEnemyTurn!");
            return;
        }
        gameState.AddTurn();
        StartPlayerTurn();
    }

    public void RemoveEnemy(GameObject enemy)
{
    if (enemies.Contains(enemy))
    {
        enemies.Remove(enemy);
        Debug.Log($"Enemy {enemy.name} removed. Remaining enemies: {enemies.Count}");
        Destroy(enemy);
    }

    if (enemies.Count == 0)
    {
        Debug.Log("All enemies defeated.");
        StartCoroutine(HandleVictoryAndStartNextBattle());
    }
}

    private void HandleVictory()
    {
        StopAllCoroutines();
        PrepareToLoadEndScene();
    }

    private void PrepareToLoadEndScene()
    {
        bannerManager.ShowBanner("Victory!");
        startAndEndGame.ShowVictoryScreen();
    }
    private IEnumerator HandleVictoryAndStartNextBattle()
{
    bannerManager.ShowBanner("Battle Won!");

    yield return new WaitForSeconds(1.5f);
    
    battleCount++;
    startAndEndGame.NextBattle();

    state = TurnState.Idle;
    yield return new WaitForSeconds(1f);
    
    state = TurnState.PlayerTurn;
    SpawnEnemies();

    StartCoroutine(StartNewPlayerTurnCycle());
}

    private IEnumerator StartNewPlayerTurnCycle()
    {
        state = TurnState.PlayerTurn;
        yield return new WaitForSeconds(1f);
        gameState.AddTurn();
        StartPlayerTurn();
    }

    public void HandleDefeat()
    {
        menuManager.HideAbilityMenu();
        bannerManager.ShowBanner("Defeat");
        gameState.YouDied();
        Debug.Log("You lost");
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        Debug.Log($"{enemy.name} added to the enemy list.");
    }

    public void SpawnEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();

        int enemyCount = battleCount;
        if (battleCount > 3)
        {
            Debug.Log("No enemies to spawn for Battle #4.");
            HandleVictory();
            return;
        }

        Debug.Log($"Spawning {enemyCount} new enemies for Battle #{battleCount}.");

        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemyCount; i++)
        {
            if (availableSpawnPoints.Count > 0)
            {
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform selectedSpawnPoint = availableSpawnPoints[spawnIndex];
                availableSpawnPoints.RemoveAt(spawnIndex);

                GameObject newEnemy = Instantiate(enemyPrefab, selectedSpawnPoint.position, Quaternion.identity);
                EnemyBehavior enemyBehavior = newEnemy.GetComponent<EnemyBehavior>();
                if (enemyBehavior != null)
                {
                    enemyBehavior.turnManager = this;
                }
                AddEnemy(newEnemy);
            }
            else
            {
                Debug.LogWarning("No available spawn points left for enemies!");
            }
        }

        Debug.Log("Enemies spawned. Transitioning to Player's turn.");
    }
}