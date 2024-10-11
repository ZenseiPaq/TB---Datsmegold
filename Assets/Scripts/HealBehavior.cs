using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHealAbility", menuName = "Ability/Heal")]
public class HealAbility : Ability
{
    public int healingAmount = 50;

    public void Use(Damageable targetDamageable)
    {
        if (targetDamageable != null)
        {
            targetDamageable.Healing(healingAmount);
            Debug.Log("Healed " + healingAmount + " health.");
        }
        else
        {
            Debug.LogWarning("TargetDamageable is null, healing not performed.");
        }
    }
}