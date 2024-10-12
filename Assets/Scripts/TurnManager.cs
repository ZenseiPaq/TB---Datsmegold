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
        StartPlayerTurn();
    }

    void StartPlayerTurn()
    {
        bannerManager.ShowBanner("Your turn");
        StartCoroutine(StartPlayerTurnWithDelay());
    }
    IEnumerator StartPlayerTurnWithDelay()
    {
        yield return new WaitForSeconds(2f);
        menuManager.ShowAbilityMenu();
        state = TurnState.PlayerTurn;
    }

    public void EndPlayerTurn()
    {
        StartEnemyTurn();        
    }

    void StartEnemyTurn()
    {
        if (enemies.Count > 0)
        {
            state = TurnState.EnemyTurn;
            Debug.Log("Enemy's Turn");
            if (currentEnemyIndex == 0)
            {
                bannerManager.ShowBanner("Their Turn");
            }

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
                enemyController.PerformMeleeAttack(); // Call the melee attack method
            }

            yield return new WaitForSeconds(1f); // Wait for enemy to finish action
        }

        EndEnemyTurn();
    }

    public void EndEnemyTurn()
    {
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
    }

    public void HandleDefeat()
    {
        menuManager.HideAbilityMenu();
        bannerManager.ShowBanner("Defeat");
        Debug.Log("You lost");
    }
}
