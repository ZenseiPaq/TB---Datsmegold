using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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
    public Damageable damageable;
    public StartAndEndGame gameState;
    public EnemyBehavior enemyBehavior;
    public int battleCount = 1;
    public Transform[] spawnPoints;
    public GameObject enemyPrefab;
    public StartAndEndGame startAndEndGame;
    private Coroutine currentCoroutine;

    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Idle,
        Endturn,
        Victory,
        Defeat
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
    private void Update()
    {
        startAndEndGame.currentBattle = battleCount;

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
        if (enemies.Count == 0)
        {
            state = TurnState.Victory;
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


    private void Start()
    {
        if (gameState == null)
        {
            gameState = FindObjectOfType<StartAndEndGame>();
            if (gameState == null)
            {
                return;
            }
        }
        battleCount++;
        StartPlayerTurn();
        SpawnEnemies();
    }

    void StartPlayerTurn()
    {
        bannerManager.ShowBanner("Your turn");
        playerController.isShieldActive = false;
        StartCoroutine(StartPlayerTurnWithDelay());
    }
    IEnumerator StartPlayerTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);
        menuManager.ShowAbilityMenu();
        state = TurnState.PlayerTurn;
        playerController.StopParticleSystem();
        playerController.CheckCurrentHP();
        playerController.isShieldActive = false;
    }
    IEnumerator EndPlayerTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);

        state = TurnState.EnemyTurn;
        StartEnemyTurn();
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

    void StartEnemyTurn()
    {
        if (enemies.Count == 0)
        {
            SpawnEnemies();
        }

        if (enemies.Count > 0)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
        else
        {
            HandleVictory();
            StopCoroutine(EnemyTurnRoutine());
        }
    }

    IEnumerator EnemyTurnRoutine()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            GameObject currentEnemy = enemies[i];

            if (currentEnemy == null)
            {
                battleCount++;
                UpdateBattleUI();
                SpawnEnemies();
                break;
            }

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
            Debug.Log($"Enemy {enemy.name} removed from the list.");
            if (currentEnemyIndex >= enemies.Count)
            {
                currentEnemyIndex--;
                if(currentEnemyIndex <= 0)
                {
                    battleCount++;
                    SpawnEnemies();
                }

            }
        }
    }
    public void HandleVictory()
    {
        StopAllCoroutines();
        bannerManager.ShowBanner("Victory");
        gameState.ShowVictoryScreen();
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
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            Debug.Log($"{enemy.name} added to the enemy list.");
        }
    }
    public void SpawnEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();

        int enemyCount = Mathf.Min(battleCount, spawnPoints.Length);

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoints[i].position, Quaternion.identity);

            EnemyBehavior enemyBehavior = newEnemy.GetComponent<EnemyBehavior>();

            if (enemyBehavior != null)
            {
                enemyBehavior.turnManager = this;
            }

            AddEnemy(newEnemy);
        }
    }

    public void UpdateBattleUI()
    {
        startAndEndGame.currentBattle = battleCount;
        startAndEndGame.ShowBattleNumber();
        if(battleCount >= 4)
        {
            HandleVictory();
        }
    }
}