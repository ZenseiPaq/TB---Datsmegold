using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Abilities/Heal")]
public class EnemyHealAbility : EnemyAbility
{
    public int healAmount;

    public EnemyHealAbility(int healAmount)
    {
        this.healAmount = healAmount;
    }

    public override void Use(GameObject target)
    {
        EnemyBehavior enemy = target.GetComponent<EnemyBehavior>();
        if (enemy != null)
        {
            enemy.Heal(healAmount);
            Debug.Log($"{abilityName} healed {enemy.name} for {healAmount} HP.");
        }
    }
}