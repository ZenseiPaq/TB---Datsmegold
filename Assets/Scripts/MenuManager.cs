using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public GameObject abilityButtonPrefab;  // Prefab for each ability button
    public Transform abilityMenuParent;     // Parent object where the buttons will be instantiated
    public List<Ability> playerAbilities;   // List of player's abilities
    public TextMeshProUGUI descriptionText;
    public TurnManager turnManager;
    public void StartTurn()
    {
        ShowAbilityMenu();
    }

    void ShowAbilityMenu()
    {
        abilityMenuParent.gameObject.SetActive(true);
        abilityButtonPrefab.SetActive(true);

        foreach (Transform child in abilityMenuParent)
        {
            Destroy(child.gameObject);  // Clear previous buttons
        }

        foreach (Ability ability in playerAbilities)
        {
            GameObject newButton = Instantiate(abilityButtonPrefab, abilityMenuParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;
            EventTrigger trigger = newButton.AddComponent<EventTrigger>();

            EventTrigger.Entry entryEnter = new EventTrigger.Entry();
            entryEnter.eventID = EventTriggerType.PointerEnter;
            entryEnter.callback.AddListener((eventData) => { ShowDescription(ability.description); });
            trigger.triggers.Add(entryEnter);
            Image iconImage = newButton.GetComponentInChildren<Image>();
            if (iconImage != null)
            {
                iconImage.sprite = ability.icon;
                iconImage.preserveAspect = true;
            }
            EventTrigger.Entry entryExit = new EventTrigger.Entry();
            entryExit.eventID = EventTriggerType.PointerExit;
            entryExit.callback.AddListener((eventData) => { HideDescription(); });
            trigger.triggers.Add(entryExit);
            newButton.GetComponent<Button>().onClick.AddListener(() => UseAbility(ability));
        }
    }
    void ShowDescription(string description)
    {
        descriptionText.gameObject.SetActive(true); 
        descriptionText.text = description;
    }

    void HideDescription()
    {
        descriptionText.gameObject.SetActive(false); 
    }
    void UseAbility(Ability ability)
    {
        ability.onUse.Invoke();  
        HideAbilityMenu();
        HideDescription();
        turnManager.EndPlayerTurn();
    }

    public void HideAbilityMenu()
    {
        abilityMenuParent.gameObject.SetActive(false);
        abilityButtonPrefab.gameObject.SetActive(false);
    }
}