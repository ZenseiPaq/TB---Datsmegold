using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    TurnManager turnManager;
    public List<EnemyAbility> abilities; // Assign up to 3 abilities via the Inspector

    public void PerformRandomAbility(GameObject target)
    {
        if (abilities.Count > 0)
        {
            int randomIndex = Random.Range(0, abilities.Count);
            abilities[randomIndex].Use(target);
        }
        turnManager.EndEnemyTurn();
    }
    
    public float moveSpeed = 2f;
    public GameObject player; 
    private Vector3 startingPosition; 
    public float attackRange = 1.5f;
    public PlayerController playerController;
    public int damage;

    private void Start()
    {

        startingPosition = transform.position;
    }

    public void PerformMeleeAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            playerController.PlayerHP(damage);
            StartCoroutine(ReturnToStartingPosition());
        }
        else
        {
            StartCoroutine(MoveToPlayer());
        }
    }

    private IEnumerator MoveToPlayer()
    {
        if (player != null)
        {
            while (Vector3.Distance(transform.position, player.transform.position) > attackRange)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
                yield return null;
            }
            PerformMeleeAttack();
            yield return new WaitForSeconds(3);
        }
    }

    private IEnumerator ReturnToStartingPosition()
    {
        // Move back to the starting position
        while (Vector3.Distance(transform.position, startingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }
    }
}

