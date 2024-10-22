using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Attack Ability", menuName = "Abilities/RangedAttack")]
public class RangedAttackAbility : EnemyAbility
{

    public override void Use(GameObject target)
    {
        EnemyBehavior enemyBehavior = target.GetComponent<EnemyBehavior>();
        enemyBehavior.ShootBullet();
    }
}

