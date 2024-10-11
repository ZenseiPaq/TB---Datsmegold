using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public int damage = 100;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Damageable damageable = other.GetComponent<Damageable>();
            damageable.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
