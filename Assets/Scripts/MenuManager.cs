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

        foreach (Transform child in abilityMenuParent)
        {
            Destroy(child.gameObject);
        }

        foreach (Ability ability in playerAbilities)
        {
            GameObject newButton = Instantiate(abilityButtonPrefab, abilityMenuParent);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = ability.abilityName;

            EventTrigger trigger = newButton.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((eventData) => ShowAbilityDescription(ability));
            trigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((eventData) => HideAbilityDescription());
            trigger.triggers.Add(pointerExit);

            newButton.GetComponent<Button>().onClick.AddListener(() => SelectAbility(ability));
        }
    }

    void SelectAbility(Ability ability)
    {
        descriptionText.enabled = false;
        playerController.SetSelectedAbility(ability);

        if (ability is HealAbility)
        {
            Debug.Log("Healing ability selected. Using on player.");
            playerController.UseAbilityOnSelf();
            return;
        }
        else if (ability is ShieldAbility)
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

    public void ShowAbilityDescription(Ability ability)
    {
        descriptionText.text = ability.description;
    }

    public void HideAbilityDescription()
    {
        descriptionText.text = "";
    }

    public void HideAbilityMenu()
    {
        abilityMenuParent.gameObject.SetActive(false);
    }
}
