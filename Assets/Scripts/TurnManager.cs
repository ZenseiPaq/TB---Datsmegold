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
    public int battleCount = 0;
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
        startAndEndGame.currentBattle = battleCount;
        battleCount++;
        StartPlayerTurn();
        SpawnEnemies();
    }

    public void StartPlayerTurn()
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

    public void StartEnemyTurn()
    {
        if (enemies.Count > 0)
        {
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    IEnumerator EnemyTurnRoutine()
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
        EnemyBehavior enemyBehavior = enemy.GetComponent<EnemyBehavior>();
        if (enemyBehavior != null)
        {
            enemyBehavior.isDefeated = true; // Mark as defeated
            enemy.tag = "Untagged"; // Optionally remove tag
        }

        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            Debug.Log($"Enemy {enemy.name} removed. Remaining enemies: {enemies.Count}");
            Destroy(enemy);
        }
        if(enemies.Count == 0)
        {            
            battleCount++;
            startAndEndGame.NextBattle();
            for(int i = 0; i<battleCount; i++)
            {
                SpawnEnemies();
                enemyBehavior.StopAllCoroutines();
            }
            StartPlayerTurn();
            state = TurnState.PlayerTurn;
        }
    }

    public void HandleVictory()
    {
        StopAllCoroutines();
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
        // Clear the existing enemies
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        enemies.Clear();

        // Calculate the number of enemies to spawn based on battle count
        int enemyCount = Mathf.Min(battleCount, 3); // Max 3 enemies
        Debug.Log($"Attempting to spawn {enemyCount} enemies.");

        List<Transform> availableSpawnPoints = new List<Transform>(spawnPoints);

        for (int i = 0; i < enemyCount; i++)
        {
            // Check if there are available spawn points
            if (availableSpawnPoints.Count > 0)
            {
                // Randomly select a spawn point and remove it from the list
                int spawnIndex = Random.Range(0, availableSpawnPoints.Count);
                Transform selectedSpawnPoint = availableSpawnPoints[spawnIndex];
                availableSpawnPoints.RemoveAt(spawnIndex);

                // Log the spawn position
                Debug.Log($"Spawning enemy at position: {selectedSpawnPoint.position}");

                // Instantiate the enemy at the selected spawn point
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
                Debug.LogWarning("No available spawn points left!");
            }
        }
    }


}