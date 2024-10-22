using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Melee Attack Ability", menuName = "Abilities/MeleeAttack")]
public class MeleeAttackAbility : EnemyAbility
{
    [SerializeField] private int damage; // Set this via the Inspector

    public override void Use(GameObject target)
    {
        PlayerController player = target.GetComponent<PlayerController>();
        if (player != null)
        {
            player.PlayerHP(damage);
            Debug.Log($"{abilityName} dealt {damage} damage to {player.name}");
        }
    }
}