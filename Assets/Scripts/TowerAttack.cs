
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class TroopAttack : MonoBehaviour
{
    public enum ProjectileType
    {
        Arrow,
        Magic,
        Bomb
    }

    [Header("Rotation")]
    public Transform troopModel;
    public float rotationSpeed = 5f;

    [Header("Targeting")]
    public Transform targetPoint;
    public string enemyTag = "Enemy";
    public float attackRange = 10f;

    [Header("Shooting")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 1f;
    public float fireDelay = 1f;
    public Animator animator;

    [Header("Projectile Type")]
    public ProjectileType projectileType;

    [Header("Model Rotation Offset")]
    [SerializeField] private float modelRotationOffset = 0f;

    private Transform currentTarget;
    private float nextFireTime;
    private bool isShooting;

    void Update()
    {
        FindClosestEnemy();

        if (currentTarget == null)
            return;

        RotateTowardsEnemy();

        Transform objectToRotate =
            troopModel != null ? troopModel : transform;

        Vector3 direction =
            currentTarget.position - objectToRotate.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        targetRotation *= Quaternion.Euler(
            0f,
            modelRotationOffset,
            0f
        );

        float angle = Quaternion.Angle(
            objectToRotate.rotation,
            targetRotation
        );

        if (!isShooting &&
            Time.time >= nextFireTime &&
            angle <= 10f)
        {
            StartCoroutine(Shoot());

            nextFireTime =
                Time.time + 1f / Mathf.Max(fireRate, 0.01f);
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);

        float closestDistanceToPoint = Mathf.Infinity;
        Transform closestEnemy = null;

        Vector3 referencePosition =
            targetPoint != null
            ? targetPoint.position
            : transform.position;

        foreach (GameObject enemy in enemies)
        {
            float distanceFromTroop = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );

            if (distanceFromTroop > attackRange)
                continue;

            float distanceToPoint = Vector3.Distance(
                referencePosition,
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

    void RotateTowardsEnemy()
    {
        if (currentTarget == null)
            return;

        Transform objectToRotate =
            troopModel != null ? troopModel : transform;

        Vector3 direction =
            currentTarget.position - objectToRotate.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        targetRotation *= Quaternion.Euler(
            0f,
            modelRotationOffset,
            0f
        );

        objectToRotate.rotation = Quaternion.RotateTowards(
            objectToRotate.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    IEnumerator Shoot()
    {
        isShooting = true;

        if (currentTarget == null ||
            projectilePrefab == null ||
            firePoint == null)
        {
            isShooting = false;
            yield break;
        }

        Transform shotTarget = currentTarget;

        if (animator != null)
            animator.SetBool("IsFiring", true);

        yield return new WaitForSeconds(fireDelay);

        if (shotTarget == null)
        {
            if (animator != null)
                animator.SetBool("IsFiring", false);

            isShooting = false;
            yield break;
        }

        GameObject projectile = Instantiate(
            projectilePrefab,
            firePoint.position,
            Quaternion.identity
        );

        switch (projectileType)
        {
            // ========================
            // BOMB
            // ========================

            case ProjectileType.Bomb:
            {
                Bomb bomb = projectile.GetComponent<Bomb>();

                if (bomb != null)
                {
                    Vector3 predictedPosition =
                        PredictEnemyPosition(
                            shotTarget,
                            bomb.TravelTime
                        );

                    bomb.SetTargetPosition(predictedPosition);
                }

                break;
            }

            // ========================
            // ARROW
            // ========================

            case ProjectileType.Arrow:
            {
                Arrow arrow = projectile.GetComponent<Arrow>();

                if (arrow != null)
                {
                    arrow.SetTarget(shotTarget);
                }

                break;
            }

            // ========================
            // MAGIC
            // ========================

            case ProjectileType.Magic:
            {
                Magic magic = projectile.GetComponent<Magic>();

                if (magic != null)
                {
                    // Get the center of the enemy's collider
                    Collider enemyCollider =
                        shotTarget.GetComponentInChildren<Collider>();

                    Vector3 targetOffset =
                        enemyCollider != null
                        ? enemyCollider.bounds.center - shotTarget.position
                        : Vector3.up * 0.5f;

                    Vector3 predictedPosition =
                        shotTarget.position;

                    // Iteratively calculate where the enemy
                    // will be when the projectile arrives
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 aimPoint =
                            predictedPosition + targetOffset;

                        float distance = Vector3.Distance(
                            firePoint.position,
                            aimPoint
                        );

                        float travelTime = CalculateTravelTime(
                            distance,
                            magic.startingSpeed,
                            magic.acceleration,
                            magic.maxSpeed
                        );

                        predictedPosition = PredictEnemyPosition(
                            shotTarget,
                            travelTime
                        );
                    }

                    Vector3 finalAimPoint =
                        predictedPosition + targetOffset;

                    Vector3 direction =
                        (finalAimPoint - firePoint.position).normalized;

                    magic.SetDirection(direction);
                }

                break;
            }
        }

        if (animator != null)
            animator.SetBool("IsFiring", false);

        isShooting = false;
    }

    // ========================
    // NAVMESH PATH PREDICTION
    // ========================

    Vector3 PredictEnemyPosition(
        Transform enemy,
        float travelTime)
    {
        NavMeshAgent agent =
            enemy.GetComponent<NavMeshAgent>();

        if (agent == null ||
            !agent.enabled ||
            !agent.isOnNavMesh ||
            !agent.hasPath ||
            agent.isStopped)
        {
            return enemy.position;
        }

        float remainingDistance =
            agent.speed * travelTime;

        Vector3 position = enemy.position;

        Vector3[] corners = agent.path.corners;

        foreach (Vector3 corner in corners)
        {
            float distance = Vector3.Distance(
                position,
                corner
            );

            if (distance <= 0.001f)
                continue;

            if (remainingDistance <= distance)
            {
                return Vector3.MoveTowards(
                    position,
                    corner,
                    remainingDistance
                );
            }

            remainingDistance -= distance;
            position = corner;
        }

        return position;
    }

    // ========================
    // PROJECTILE TRAVEL TIME
    // ========================

    float CalculateTravelTime(
        float distance,
        float startingSpeed,
        float acceleration,
        float maxSpeed)
    {
        startingSpeed = Mathf.Max(startingSpeed, 0.01f);
        maxSpeed = Mathf.Max(maxSpeed, 0.01f);

        if (acceleration <= 0f || startingSpeed >= maxSpeed)
        {
            return distance / Mathf.Min(startingSpeed, maxSpeed);
        }

        float timeToMaxSpeed =
            (maxSpeed - startingSpeed) / acceleration;

        float distanceWhileAccelerating =
            startingSpeed * timeToMaxSpeed +
            0.5f * acceleration *
            timeToMaxSpeed * timeToMaxSpeed;

        if (distance <= distanceWhileAccelerating)
        {
            float discriminant =
                startingSpeed * startingSpeed +
                2f * acceleration * distance;

            return
                (-startingSpeed + Mathf.Sqrt(discriminant))
                / acceleration;
        }

        float remainingDistance =
            distance - distanceWhileAccelerating;

        return timeToMaxSpeed +
               remainingDistance / maxSpeed;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            attackRange
        );
    }
    void Start()
{
    TroopRangeVisualizer rangeVisualizer =
        GetComponent<TroopRangeVisualizer>();

    if (rangeVisualizer != null)
    {
        rangeVisualizer.SetRange(attackRange);
    }
}
}
