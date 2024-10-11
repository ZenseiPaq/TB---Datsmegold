using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public TurnManager turnManager;

    private void Start()
    {
        maxHP = 100;
        currentHP = maxHP;
    }
    public void TakeDamage(int damage)
    {
        Debug.Log("I took: " + damage);
        currentHP = currentHP - damage;
        if(currentHP <= 0 )
        {
            Destroy(gameObject);
            turnManager.RemoveEnemy(gameObject);
        }
    }
}
