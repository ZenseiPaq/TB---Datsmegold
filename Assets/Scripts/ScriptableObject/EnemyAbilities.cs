using UnityEngine;

public abstract class EnemyAbility : ScriptableObject
{
    public string abilityName;
    public abstract void Use(GameObject target);
}

