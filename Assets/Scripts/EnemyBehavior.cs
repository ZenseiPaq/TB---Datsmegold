using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public List<EnemyAbility> abilities;
    public Transform shootPoint;
    public GameObject bulletPrefab;

    // Character models
    public GameObject archerModel;
    public GameObject warlockModel;
    public GameObject barbarianModel;
    public GameObject fighterModel;

    // Character attributes
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;
    public int meleeDamage;
    public int healAmount;

    public PlayerController player;
    private Vector3 startingPosition;
    public float attackRange = 1.5f;
    public GameObject healParticle;
    public TurnManager turnManager;

    private void Start()
    {
        startingPosition = transform.position;

        SelectRandomCharacter();

        currentHealth = maxHealth;
    }

    private void SelectRandomCharacter()
    {
        int randomCharacter = Random.Range(0, 4);

        switch (randomCharacter)
        {
            case 0:
                SetupCharacterOne();
                break;
            case 1:
                SetupCharacterTwo();
                break;
            case 2:
                SetupCharacterThree();
                break;
            case 3:
                SetupCharacterFour();
                break;
        }
    }

    private void SetupCharacterOne()
    {
        Instantiate(fighterModel, transform.position, Quaternion.identity, transform);

        maxHealth = 150;
        moveSpeed = 9f;
        meleeDamage = 25;
        healAmount = 30;
        Debug.Log("Character One selected with 150 HP, 2.5 speed, 25 melee damage, and 30 heal amount.");
    }

    private void SetupCharacterTwo()
    {
        Instantiate(archerModel, transform.position, Quaternion.identity, transform);

        maxHealth = 100;
        moveSpeed = 8f;
        meleeDamage = 20;
        healAmount = 25;
        Debug.Log("Character Two selected with 100 HP, 3 speed, 20 melee damage, and 25 heal amount.");
    }

    private void SetupCharacterThree()
    {
        Instantiate(warlockModel, transform.position, Quaternion.identity, transform);

        maxHealth = 120;
        moveSpeed = 7f;
        meleeDamage = 30;
        healAmount = 35;
        Debug.Log("Character Three selected with 120 HP, 2 speed, 30 melee damage, and 35 heal amount.");
    }

    private void SetupCharacterFour()
    {
        Instantiate(barbarianModel, transform.position, Quaternion.identity, transform);

        maxHealth = 200;
        moveSpeed = 6f;
        meleeDamage = 40;
        healAmount = 40;
        Debug.Log("Character Four selected with 200 HP, 1.5 speed, 40 melee damage, and 40 heal amount.");
    }

    public void PerformRandomAbility(GameObject target)
    { 
        if (currentHealth < 40)
        {
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
        Destroy(healEffect, 4f);
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
        while (Vector3.Distance(transform.position, startingPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startingPosition, moveSpeed * Time.deltaTime);
            yield return null;
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
            Destroy(this.gameObject);
        }
        Debug.Log($"{gameObject.name} took {damage} damage and has {currentHealth} HP left.");
    }
}