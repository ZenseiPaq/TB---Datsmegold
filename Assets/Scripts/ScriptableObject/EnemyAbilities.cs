using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyAbility", menuName = "Ability/EnemyAbility")]
public class EnemyAbility : ScriptableObject
{
    public string abilityName;
    public int damage;
    public virtual void Use(GameObject target)
    {
        Debug.Log($"Enemy used {abilityName} on {target.name}");
    }
}

public class RangedAttack : EnemyAbility
{
    public override void Use(GameObject target)
    {
        base.Use(target);
        Debug.Log("Ranged attack performed on " + target.name);
    }
}

public class EnemyHealAbility : EnemyAbility
{
    public int healAmount = 20; // Amount to heal
    public float healingRange = 5f; // Range to find allies for healing

    public override void Use(GameObject target)
    {
        EnemyBehavior enemy = target.GetComponent<EnemyBehavior>();
        if (enemy != null)
        {
            // Heal the enemy
            enemy.Heal(healAmount);
            Debug.Log($"{enemy.name} healed for {healAmount} HP");
        }
    }

    public void HealNearbyAllies(EnemyBehavior enemy)
    {
        // Find allies within healing range
        Collider[] hitColliders = Physics.OverlapSphere(enemy.transform.position, healingRange);
        foreach (var hitCollider in hitColliders)
        {
            EnemyBehavior ally = hitCollider.GetComponent<EnemyBehavior>();
            if (ally != null && ally != enemy) // Don't heal self
            {
                ally.Heal(healAmount);
                Debug.Log($"{ally.name} healed for {healAmount} HP");
            }
        }
    }
}
