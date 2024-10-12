using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability/Gunshot")]
public class GunshotAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 2f;
    public Transform SelectedEnemy;
    

    public void Use(Transform target)
    {
        if (projectilePrefab != null)
        {
            Transform gunTransform = target.transform;
            Vector3 startingPosition = gunTransform.position;
            GameObject projectile = Instantiate(projectilePrefab, startingPosition, Quaternion.identity);
            Vector3 direction = (target.position - projectile.transform.position).normalized;

            // Set the projectile's speed
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
        }
    }
}
