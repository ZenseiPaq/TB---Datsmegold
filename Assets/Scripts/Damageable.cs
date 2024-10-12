using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public TurnManager turnManager;
    public GameObject player;

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
    public void Healing(int damage)
    {
        currentHP = currentHP + damage;
    }

    public void RemoveOverHealth()
    {
        if(currentHP > 100)
        {
            currentHP = 100;
        }
    }
    private void HandleDeath()
    {
        turnManager.RemoveEnemy(gameObject);
    }
}
