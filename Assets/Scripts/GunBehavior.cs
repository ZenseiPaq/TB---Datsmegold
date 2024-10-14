using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability/Gunshot")]
public class GunshotAbility : Ability
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 2f;

    public void Use(Transform shootPoint, Transform target)
    {
        if (projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

            Vector3 direction = (target.position - shootPoint.position).normalized;
            projectile.transform.rotation = Quaternion.LookRotation(direction);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed; 
            }
        }
    }
}
