using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TowerAttack : MonoBehaviour
{
    public enum ProjectileType
    {
        Arrow,
        Magic,
        Bomb
    }

    [Header("Targeting")]
    public Transform targetPoint;
    public string enemyTag = "Enemy";
    public float attackRange = 10f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float fireDelay=1;
    public Animator animator;

    [Header("Projectile Script")]
    public ProjectileType projectileType;

    private Transform currentTarget;
    private float nextFireTime;

    void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null && Time.time >= nextFireTime)
        {
            StartCoroutine(Shoot());
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
            float distanceFromTower = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distanceFromTower > attackRange)
                continue;

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

     IEnumerator Shoot()
    {
        if (currentTarget == null)
            yield break;
        animator.SetBool("Shoot",true);
        yield return new WaitForSeconds(fireDelay);
        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        switch (projectileType)
        {
            case ProjectileType.Bomb:

    Bomb bomb = projectile.GetComponent<Bomb>();

    if (bomb != null)
    {
        Vector3 predictedPosition = currentTarget.position;

        NavMeshAgent enemyAgent =
            currentTarget.GetComponent<NavMeshAgent>();

        if (enemyAgent != null)
        {
            predictedPosition =
                currentTarget.position +
                enemyAgent.velocity * bomb.TravelTime;
        }

        bomb.SetTargetPosition(predictedPosition);
    }

    break;

            // ========================
            // ARROW
            // ========================

            case ProjectileType.Arrow:

                Arrow arrow = projectile.GetComponent<Arrow>();

                if (arrow != null)
                {
                    arrow.SetTarget(currentTarget);
                }

                break;


            // ========================
            // MAGIC
            // ========================

            case ProjectileType.Magic:

                Magic magic = projectile.GetComponent<Magic>();

                if (magic != null)
                {
                    Vector3 predictedPosition = currentTarget.position;

                    NavMeshAgent enemyAgent =
                        currentTarget.GetComponent<NavMeshAgent>();

                    if (enemyAgent != null)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            float distance = Vector3.Distance(
                                firePoint.position,
                                predictedPosition
                            );

                            float travelTime = CalculateTravelTime(
                                distance,
                                magic.startingSpeed,
                                magic.acceleration,
                                magic.maxSpeed
                            );

                            predictedPosition =
                                currentTarget.position +
                                enemyAgent.velocity * travelTime;
                        }
                    }

                    Vector3 direction =
                        (predictedPosition - firePoint.position).normalized;

                    magic.SetDirection(direction);
                }

                break;
        }
        animator.SetBool("IsFiring",false);
    }

    float CalculateTravelTime(
        float distance,
        float startingSpeed,
        float acceleration,
        float maxSpeed)
    {
        if (acceleration <= 0f)
        {
            return distance / Mathf.Max(startingSpeed, 0.01f);
        }

        float timeToMaxSpeed =
            (maxSpeed - startingSpeed) / acceleration;

        timeToMaxSpeed = Mathf.Max(0f, timeToMaxSpeed);

        // How far it travels while accelerating
        float distanceWhileAccelerating =
            startingSpeed * timeToMaxSpeed +
            0.5f * acceleration *
            timeToMaxSpeed * timeToMaxSpeed;

        // Target is reached BEFORE max speed
        if (distance <= distanceWhileAccelerating)
        {
            // Solve:
            //
            // distance = startingSpeed * t
            //          + 0.5 * acceleration * t²

            float discriminant =
                startingSpeed * startingSpeed +
                2f * acceleration * distance;

            return
                (-startingSpeed + Mathf.Sqrt(discriminant))
                / acceleration;
        }

        else
        {
            float remainingDistance =
                distance - distanceWhileAccelerating;

            float timeAtMaxSpeed =
                remainingDistance / maxSpeed;

            return timeToMaxSpeed + timeAtMaxSpeed;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
}