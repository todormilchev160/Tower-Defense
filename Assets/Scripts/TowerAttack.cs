using UnityEngine;

public class TowerAttack : MonoBehaviour
{
    [Header("Targeting")]
    public Transform targetPoint;
    public string enemyTag = "Enemy";
    public float attackRange = 10f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private Transform currentTarget;
    private float nextFireTime;

    void Update()
    {
        // Find closest enemy that is inside range
        FindClosestEnemy();

        // Shoot only if we have a valid target
        if (currentTarget != null && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float closestDistanceToPoint = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // Distance from tower to enemy
            float distanceFromTower = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            // Ignore enemies outside tower range
            if (distanceFromTower > attackRange)
                continue;

            // Find which enemy is closest to your target point
            float distanceToPoint = Vector3.Distance(
                targetPoint.position,
                enemy.transform.position
            );

            if (distanceToPoint < closestDistanceToPoint)
            {
                closestDistanceToPoint = distanceToPoint;
                closestEnemy = enemy.transform;
            }
        }

        currentTarget = closestEnemy;
    }

    void Shoot()
    {
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        Projectile projectileScript = projectile.GetComponent<Projectile>();

        if (projectileScript != null)
        {
            projectileScript.SetTarget(currentTarget);
        }
    }

    // Shows the tower range in the Scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}