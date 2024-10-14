using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Playables;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
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
    private void Start()
    {
        playerOverHealth.Stop();
        shieldPrefab.SetActive(false);
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            RotateTowardsEnemy(targetEnemy);
        }
        if(playerHealth < 100 && playerHealth > 0)
        {
            StopParticleSystem();
        }
        if(isShieldActive == true)
        {
            shieldPrefab.SetActive(true); 
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

        // Clear previous buttons
        foreach (Transform child in buttonParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            GameObject enemy = enemies[i];
            GameObject newButton = Instantiate(enemyButtonPrefab, buttonParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = enemy.name;
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -30 * i);
            GameObject localEnemy = enemy;

            // Add listener to select enemy and use ability
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectEnemy(localEnemy.transform));
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
            gunshotAbility.Use(targetEnemy);
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
            turnManager.EndPlayerTurn();
        }
        else if(selectedAbility is ShieldAbility shieldAbility)
        {
            shieldAbility.ActivateShield(this);
        }
    }

    void RotateTowardsEnemy(Transform enemy)
    {
        Vector3 direction = (enemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }


    public void PlayerHP(int damage)
    {

        if (isShieldActive)
        {
            damage = Mathf.Max(0, damage / 2);
            playerHealth = playerHealth - damage;
        }
        else
        {
            playerHealth = playerHealth - damage;
            Debug.Log("I took" + damage);
        }

        if (playerHealth <= 0)
        {
            turnManager.HandleDefeat();
        }

        if (playerHealth > 100)
        {
            playerOverHealth.Play();
        }
    }
    public void CheckCurrentHP()
    {
        shieldPrefab.SetActive(false);
        if (playerHealth > maxHealth)
        {
            playerHealth = maxHealth;
        }
    }
    public void StopParticleSystem()
    {
        playerOverHealth.Stop();
    }

    public void Heal(int healing)
    {
        playerHealth += healing;
    }
    public void SetShieldActive(int shieldPower)
    {
        isShieldActive = true;
        shieldDamageReduction = shieldPower;
        shieldPrefab.SetActive(true);
        turnManager.EndPlayerTurn();
        Debug.Log("ending turn with shield");
    }
}
    
