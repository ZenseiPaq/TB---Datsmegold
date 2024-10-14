using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    
    public GameObject abilityButtonPrefab;  
    public Transform abilityMenuParent;    
    public List<Ability> playerAbilities;   
    public TextMeshProUGUI descriptionText;
    public PlayerController playerController; 

    public void ShowAbilityMenu()
    {
        abilityMenuParent.gameObject.SetActive(true);

        // Clear previous buttons
        foreach (Transform child in abilityMenuParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Ability ability in playerAbilities)
        {
            GameObject newButton = Instantiate(abilityButtonPrefab, abilityMenuParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;

            // Add a listener to handle ability selection
            newButton.GetComponent<Button>().onClick.AddListener(() => SelectAbility(ability));
        }
    }

    void SelectAbility(Ability ability)
    {
        playerController.SetSelectedAbility(ability);

        if (ability is HealAbility)
        {
            Debug.Log("Healing ability selected. Using on player.");
            playerController.UseAbilityOnSelf();
            return; 
        }
        else if(ability is ShieldAbility)
        {
            playerController.UseAbilityOnSelf();
            return;
        }
        else
        {
            playerController.CreateEnemyButtons();
        }
        HideAbilityMenu();
    }

    public void HideAbilityMenu()
    {
        abilityMenuParent.gameObject.SetActive(false);
    }
}