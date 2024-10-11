using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public CanvasGroup bannerCanvasGroup;
    public TurnState state; 
    public List<GameObject> players; 
    public List<GameObject> enemies;  
    private int currentEnemyIndex = 0;
    public BannerManager bannerManager;

    private Coroutine currentCoroutine;
    public enum TurnState
    {
        PlayerTurn,
        EnemyTurn,
        Idle,
        Endturn
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
    }
    private void Start()
    {
        Debug.Log("Turn Manager Initialized");
        StartPlayerTurn();
    }
    void StartPlayerTurn()
    {
        Debug.Log("Player's Turn");
        state = TurnState.PlayerTurn;
    }

   public void EndPlayerTurn()
    {        
        StartEnemyTurn();
        bannerManager.ShowBanner("Their Turn");
    }

    void StartEnemyTurn()
    {
        state = TurnState.EnemyTurn;
        Debug.Log("Enemy's Turn");
        
    }

    public void EndEnemyTurn()
    {
        currentEnemyIndex++;
        if (currentEnemyIndex >= enemies.Count)
        {
            currentEnemyIndex = 0;
            StartPlayerTurn();
            bannerManager.ShowBanner("Your Turn");
        }
        else
        {
            StartEnemyTurn();           
        }
    }
    void CheckCombatEnd()
    {
        if (players.Count == 0)
        {
        }
        else if (enemies.Count == 0)
        {
        }
    }

}
