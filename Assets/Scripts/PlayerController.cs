using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    public float rotationSpeed = 5f;
    private Transform targetEnemy;
    public GameObject enemyButtonPrefab;
    public Transform buttonParent;
    private Ability selectedAbility;
    public TurnManager turnManager;
    public HealAbility Healing;
    public GameObject player;
    public int playerHealth;
    public int startHealth = 100;
    public int maxHealth = 100;
    public ParticleSystem playerOverHealth;
    public bool isShieldActive = false;
    private int shieldDamageReduction;
    public GameObject shieldPrefab;
    public TextMeshProUGUI healthDisplay;
    public Transform shootPoint;
    public StartAndEndGame startEndGame;

    private void Start()
    {
        playerHealth = startHealth;
        UpdateHealthDisplay();
        playerOverHealth.Stop();
        shieldPrefab.SetActive(false);
        startEndGame = FindObjectOfType<StartAndEndGame>();
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

    void Update()
    {
        if (targetEnemy != null)
        {
            RotateTowardsEnemy(targetEnemy);
        }
        if (playerHealth < 100 && playerHealth > 0)
        {
            StopParticleSystem();
        }
        if (isShieldActive)
        {
            shieldPrefab.SetActive(true);
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            startEndGame.EscapeMainMenu();
        }
    }

    public void SetSelectedAbility(Ability ability)
    {
        selectedAbility = ability;
        Debug.Log("Selected Ability: " + selectedAbility.abilityName);
    }

    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
        Debug.Log("Selected Ability: " + selectedAbility.abilityName);

        if (selectedAbility is HealAbility)
        {
            UseAbilityOnSelf();
            return;
        }
        else if (selectedAbility is ShieldAbility shieldAbility)
        {
            shieldAbility.ActivateShield(this);
            return;
        }
        else
        {
            CreateEnemyButtons();
        }
    }

    public void CreateEnemyButtons()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        buttonParent.gameObject.SetActive(true);

        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            EnemyBehavior enemyBehavior = enemies[i].GetComponent<EnemyBehavior>();
            if (enemyBehavior != null && !enemyBehavior.isDefeated)
            {
                GameObject enemy = enemies[i];
                GameObject newButton = Instantiate(enemyButtonPrefab, buttonParent);
                newButton.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name;
                newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * i);
                GameObject localEnemy = enemy;

                newButton.GetComponent<Button>().onClick.AddListener(() => SelectEnemy(localEnemy.transform));
            }
        }
    }

    public void SelectEnemy(Transform enemy)
    {
        targetEnemy = enemy;
        Debug.Log("Enemy selected: " + targetEnemy.name);
        UseAbilityOnTarget();
    }

    void UseAbilityOnTarget()
    {
        if (selectedAbility is GunshotAbility gunshotAbility && targetEnemy != null)
        {
            gunshotAbility.Use(shootPoint.transform, targetEnemy);
        }
        targetEnemy = null;
        turnManager.EndPlayerTurn();
        buttonParent.gameObject.SetActive(false);
    }

    public void UseAbilityOnSelf()
    {
        if (selectedAbility is HealAbility healAbility)
        {
            healAbility.Use(this);
            UpdateHealthDisplay();
            turnManager.EndPlayerTurn();
        }
        else if (selectedAbility is ShieldAbility shieldAbility)
        {
            shieldAbility.ActivateShield(this);
        }
    }

    void RotateTowardsEnemy(Transform enemy)
    {
        Vector3 direction = (enemy.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void PlayerHP(int damage)
    {
        if (isShieldActive)
        {
            damage = Mathf.Max(0, damage / 2);
            playerHealth -= damage;
        }
        else
        {
            playerHealth -= damage;
            Debug.Log("I took " + damage + " damage");
        }

        if (playerHealth <= 0)
        {
            turnManager.HandleDefeat();
        }

        UpdateHealthDisplay();

        if (playerHealth > maxHealth)
        {
            playerOverHealth.Play();
        }
    }

    public void UpdateHealthDisplay()
    {
        healthDisplay.text = $"Health: {playerHealth}/{maxHealth}";


    }

    public void CheckCurrentHP()
    {
        shieldPrefab.SetActive(false);
        if (playerHealth > maxHealth)
        {
            playerHealth = maxHealth;
            UpdateHealthDisplay();
        }
    }

    public void StopParticleSystem()
    {
        playerOverHealth.Stop();
    }

    public void Heal(int healing)
    {
        playerHealth += healing;
        UpdateHealthDisplay();
    }

    public void SetShieldActive(int shieldPower)
    {
        isShieldActive = true;
        shieldDamageReduction = shieldPower;
        shieldPrefab.SetActive(true);
        turnManager.EndPlayerTurn();
        Debug.Log("Ending turn with shield");
    }
}
