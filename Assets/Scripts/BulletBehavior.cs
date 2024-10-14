using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 10f;  // Speed of the bullet
    public int pDamage = 10;     // Damage dealt by the bullet
    public int eDamage = 20;
    private Vector3 direction;   

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;  
        Destroy(gameObject, 3f); 
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.PlayerHP(pDamage);
                Destroy(gameObject);
                Debug.Log("Player damage");
            }
            Destroy(gameObject); 
        }
        if (collider.gameObject.CompareTag("Enemy")) 
        {
            EnemyBehavior enemyBehavior = collider.gameObject.GetComponent<EnemyBehavior>();
            enemyBehavior.EnemyTakeDamage(eDamage);
            Destroy(gameObject);
            Debug.Log("EnemyTakeDamage");
        }
    }
}
