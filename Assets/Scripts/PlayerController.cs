using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor.Playables;

public class PlayerController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    private Transform targetEnemy;
    public GameObject enemyButtonPrefab;
    public Transform buttonParent;
    private Ability selectedAbility;
    public TurnManager turnManager;
    public HealAbility Healing;
    public Damageable playerDamageable;

    private void Start()
    {
        playerDamageable = GetComponent<Damageable>();
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            RotateTowardsEnemy(targetEnemy);
        }
    }

    public void SetSelectedAbility(Ability ability)
    {
        selectedAbility = ability; // Set the selected ability
        Debug.Log("Selected Ability: " + selectedAbility.abilityName);
    }

    public void SelectAbility(Ability ability)
    {
        selectedAbility = ability;
        Debug.Log("Selected Ability: " + selectedAbility.abilityName);

        if (selectedAbility is HealAbility)
        {
            UseAbilityOnSelf();
            return; // Early exit to prevent creating enemy buttons
        }
        else
        {
            CreateEnemyButtons(); // Create buttons only for non-heal abilities
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
        UseAbilityOnTarget(); // Call to use the selected ability on the target
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
            healAbility.Use(playerDamageable);
            turnManager.EndPlayerTurn();
        }
    }

    void RotateTowardsEnemy(Transform enemy)
    {
        Vector3 direction = (enemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }
}
