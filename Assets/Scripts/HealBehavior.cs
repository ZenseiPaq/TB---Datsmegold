using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Ability/Heal")]
public class HealAbility : Ability
{
    public int healingAmount = 50;
    
    public void Use(PlayerController playerController)
    {
        playerController.Heal(healingAmount);
    }
}