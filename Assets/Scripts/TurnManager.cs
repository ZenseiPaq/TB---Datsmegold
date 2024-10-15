using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
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

    private void Update()
    {
        // Check for input to end turns (this can be expanded based on your game's needs)
        if (Input.GetKeyDown(KeyCode.W))
        {
            EndPlayerTurn();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndEnemyTurn();
        }

        // Check for victory or defeat
        if (players.Count == 0)
        {
            state = TurnState.Defeat;
        }
        if (enemies.Count == 0)
        {
            state = TurnState.Victory;
        }

        // Trigger actions based on the current state
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
        }
        
        StartPlayerTurn();
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
                Debug.LogWarning("Enemy at index " + i + " is null, skipping.");
                continue;
            }

            EnemyBehavior enemyController = currentEnemy.GetComponent<EnemyBehavior>();
            if (enemyController != null)
            {
                // Check if the enemy should heal
                bool useHeal = enemyController.currentHealth < 40;

                if (useHeal)
                {
                    enemyController.HealSelf(); // Heal self
                    Debug.Log($"{enemyController.name} heals.");
                }
                else
                {
                    // Decide randomly between ranged and melee attack
                    bool useRangedAttack = Random.Range(0f, 1f) > 0.5f; // 50% chance for ranged attack

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

            yield return new WaitForSeconds(2f); // Wait for enemy to finish action
        }

        EndEnemyTurn();
    }
    public void EndEnemyTurn()
    {
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
                Debug.Log("Enemies remaining" + enemies.Count);
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
}
