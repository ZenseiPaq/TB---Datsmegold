using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewShieldAbility", menuName = "Ability/Shield")]
public class ShieldAbility : Ability
{
    public int shieldPower;

    public void ActivateShield(PlayerController player)
    {
        player.SetShieldActive(shieldPower);
    }
}
