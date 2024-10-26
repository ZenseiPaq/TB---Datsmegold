using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public List<EnemyAbility> abilities;
    public Transform shootPoint;
    public GameObject bulletPrefab;
    public GameObject LazerObject;
    public string characterName;

    // Character models
    public GameObject archerModel;
    public GameObject warlockModel;
    public GameObject barbarianModel;
    public GameObject fighterModel;
    private GameObject instantiatedModel;
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
    public GameObject WarlockSpell;
    public GameObject ArcherSpell;
    public bool isDefeated = false;
    // Serialized fields to assign ScriptableObject abilities from the editor
    [SerializeField] private EnemyAbility meleeAttackAbility;
    [SerializeField] private EnemyAbility rangedAttackAbility;
    [SerializeField] private EnemyAbility healAbility;
    public enum CharacterType { Archer, Warlock, Barbarian, Fighter }
    public CharacterType characterType;

    private void Start()
    {
        
            startingPosition = transform.position;

            player = FindObjectOfType<PlayerController>();
            if (player == null)
            {
                Debug.LogError("PlayerController not found in the scene!");
            }
            SelectRandomCharacter();
            currentHealth = maxHealth;
            
    }

    private void SelectRandomCharacter()
    {
        int randomCharacter = Random.Range(0, 4);

        switch (randomCharacter)
        {
            case 0:
                characterType = CharacterType.Fighter;
                SetupFighter();
                break;
            case 1:
                characterType = CharacterType.Archer;
                SetupArcher();
                break;
            case 2:
                characterType = CharacterType.Warlock;
                SetupWarlock();
                break;
            case 3:
                characterType = CharacterType.Barbarian;
                SetupBarbarian();
                break;
        }
    }

    private void SetupFighter()
    {
        if (fighterModel != null)
        {
            instantiatedModel = Instantiate(fighterModel, transform.position, Quaternion.identity, transform);
            maxHealth = 150;
            moveSpeed = 9f;
            meleeDamage = 25;
            healAmount = 30;
            abilities = new List<EnemyAbility> { meleeAttackAbility, healAbility };
            characterName = "Fighter";
            Debug.Log("Fighter selected with 150 HP, 9 speed, 25 melee damage, and 30 heal amount.");
        }
    }

    private void SetupArcher()
    {
        if (archerModel != null)
        {
            instantiatedModel = Instantiate(archerModel, transform.position, Quaternion.identity, transform);
            maxHealth = 100;
            moveSpeed = 8f;
            meleeDamage = 20;
            healAmount = 25;
            characterName = "Archer";
            abilities = new List<EnemyAbility> { rangedAttackAbility, healAbility };
            Debug.Log("Archer selected with 100 HP, 8 speed, 20 melee damage, and 25 heal amount.");
        }
    }

    private void SetupWarlock()
    {
        if (warlockModel != null)
        {
            instantiatedModel = Instantiate(warlockModel, transform.position, Quaternion.identity, transform);
            maxHealth = 120;
            moveSpeed = 7f;
            meleeDamage = 30;
            healAmount = 35;
            characterName = "Warlock";
            abilities = new List<EnemyAbility> { rangedAttackAbility, healAbility };
            Debug.Log("Warlock selected with 120 HP, 7 speed, 30 melee damage, and 35 heal amount.");
        }
    }

    private void SetupBarbarian()
    {
        if (barbarianModel != null)
        {
            instantiatedModel = Instantiate(barbarianModel, transform.position, Quaternion.identity, transform);
            maxHealth = 200;
            moveSpeed = 6f;
            meleeDamage = 40;
            healAmount = 40;
            characterName = "Barbarian";
            abilities = new List<EnemyAbility> { meleeAttackAbility };
            Debug.Log("Barbarian selected with 200 HP, 6 speed, 40 melee damage, and 40 heal amount.");
        }
    }

    public void PerformRandomAbility(GameObject target)
    {
        if (abilities.Count > 0)
        {

            {
                int randomIndex = Random.Range(0, abilities.Count);
                EnemyAbility selectedAbility = abilities[randomIndex];
                selectedAbility.Use(target);
                Debug.Log($"{gameObject.name} uses ability: {selectedAbility.abilityName}");
            }
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
        if (characterType == CharacterType.Warlock)
        {
            PerformRangedAttack();
            return;
        }
        
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

    public void ShootBullet()
    {
        if (characterType == CharacterType.Warlock)
        {
            GameObject bullet = Instantiate(WarlockSpell, shootPoint.position, Quaternion.identity);
            Vector3 direction = (player.transform.position - shootPoint.position).normalized;
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            bulletBehavior.Initialize(direction);
        }
        if(characterType == CharacterType.Archer)
        {
            GameObject bullet = Instantiate(ArcherSpell, shootPoint.position, Quaternion.identity);
            Vector3 direction = (player.transform.position - shootPoint.position).normalized;
            BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
            bulletBehavior.Initialize(direction);
        }
        if (characterType == CharacterType.Fighter)
        {
            PerformMeleeAttack();
        }
        if (characterType == CharacterType.Barbarian)
        {
            PerformMeleeAttack();
        }
    }

    public void EnemyTakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // Destroy all child objects (e.g., models)
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            StartCoroutine(CheckIfDefeated());
            Debug.Log($"{gameObject.name} has been defeated.");
        }
        Debug.Log($"{gameObject.name} took {damage} damage and has {currentHealth} HP left.");
    }
    private IEnumerator CheckIfDefeated()
    {
        // Wait a frame to ensure children are destroyed
        yield return null;

        // Check if all children are destroyed
        if (transform.childCount == 0)
        {
            Debug.Log($"{gameObject.name} has no remaining models and is considered fully defeated.");
            turnManager.RemoveEnemy(gameObject);
        }
    }
}