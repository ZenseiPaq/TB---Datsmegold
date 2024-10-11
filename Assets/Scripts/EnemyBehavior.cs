using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public void TakeTurn()
    {
        Debug.Log("Enemy takes its turn.");
        EndTurn();
    }

    public void EndTurn()
    {
        FindObjectOfType<TurnManager>().EndEnemyTurn();
    }
}
