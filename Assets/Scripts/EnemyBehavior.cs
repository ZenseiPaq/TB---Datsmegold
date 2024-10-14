using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public List<EnemyAbility> abilities;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    public int maxHealth = 100;
    public int currentHealth;
    public PlayerController player;
    private Vector3 startingPosition;
    public float attackRange = 1.5f;
    public float moveSpeed = 2f;
    public int meleeDamage = 20; // Damage dealt by melee attacks
    public int healAmount = 25; // Amount to heal
    public GameObject healParticle; // Healing particle prefab
    public TurnManager turnManager;

    private void Start()
    {
        currentHealth = maxHealth;
        startingPosition = transform.position;
    }

    public void PerformRandomAbility(GameObject target)
    {
        // Check if the enemy needs healing
        if (currentHealth < 40)
        {
            // Randomly decide to heal 60% of the time
            if (Random.Range(0f, 1f) < 0.6f)
            {
                HealSelf();
                Debug.Log($"{gameObject.name} heals itself.");
                return;
            }
        }
        if (abilities.Count > 0)
        {
            int randomIndex = Random.Range(0, abilities.Count);
            abilities[randomIndex].Use(target);
            Debug.Log($"{gameObject.name} uses ability: {abilities[randomIndex].name}");
        }

    }

    public void HealSelf()
    {
        Heal(healAmount);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        InstantiateHealEffect();
        Debug.Log($"{gameObject.name} healed to {currentHealth} HP");
    }

    private void InstantiateHealEffect()
    {
        GameObject healEffect = Instantiate(healParticle, transform.position, Quaternion.identity);
        Destroy(healEffect, 4f); // Destroy the particle effect after 4 seconds
    }

    public void PerformMeleeAttack()
    {
        if (Vector3.Distance(transform.position, player.transform.position) <= attackRange)
        {
            player.PlayerHP(meleeDamage);
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

    public void PerformRangedAttack()
    {
        ShootBullet();
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Vector3 direction = (player.transform.position - shootPoint.position).normalized;
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        bulletBehavior.Initialize(direction);
    }

    public void EnemyTakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0 )
        {
            turnManager.RemoveEnemy(gameObject);
        }
        Debug.Log($"{gameObject.name} took {damage} damage and has {currentHealth} HP left.");
    }
}