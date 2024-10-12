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

public class MeleeAttack : EnemyAbility
{
    public override void Use(GameObject target)
    {
        base.Use(target);
        Debug.Log("Melee attack performed on " + target.name);
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
