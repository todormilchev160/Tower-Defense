
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Troops : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 20f;
    [SerializeField] private Image healthbarfill;

    private float health;
    private Barracks barracks;

    [Header("Combat")]
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackInterval = 1f;
    [SerializeField] private float engagementDistance = 2f;

    [Header("Targeting")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float detectionRange = 10f;

    [Header("Rally Point")]
    [SerializeField] private float rallyStoppingDistance = 1f;

    [Header("Rotation")]
    [SerializeField] private Transform troopModel;
    [SerializeField] private float rotationSpeed = 5f;

    private NavMeshAgent agent;
    private EnemyCombat currentEnemy;
    private EnemyHealth currentEnemyHealth;
    private Animator animator;

    private bool engaged = false;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        health = maxHealth;

        if (agent != null)
        {
            agent.avoidancePriority = 50;
            agent.obstacleAvoidanceType =
                ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        }
    }

    void Update()
    {
        if (healthbarfill != null)
        {
            healthbarfill.fillAmount =
                health / maxHealth;
        }

        if (isDead)
            return;

        RotateTowardsEnemy();

        if (engaged)
            return;

    
            FindNearestEnemy();

            if (currentEnemy == null)
            {
                GoToRallyPoint();
                return;
            }
        

        if (currentEnemy.IsDead() ||
            currentEnemy.IsEngaged())
        {
            ClearEnemy();
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            currentEnemy.transform.position
        );

        if (distance <= engagementDistance)
        {
            StartFight();
        }
        else
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    currentEnemy.transform.position
                );
            }

            if (animator != null)
            {
                animator.SetBool("Walk", true);
            }
        }
    }

    // ==========================================
    // NAVMESH COMBAT CONTROL
    // ==========================================

    void StopForCombat()
    {
        if (agent == null || !agent.enabled)
            return;

        if (agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        // Remove troop from NavMesh avoidance
        agent.enabled = false;
    }

    void ResumeAfterCombat()
    {
        if (agent == null)
            return;

        if (!agent.enabled)
            agent.enabled = true;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "Troop could not resume: not on NavMesh!"
            );
            return;
        }

        agent.isStopped = false;
    }

    // ==========================================
    // BARRACKS
    // ==========================================

    public void SetBarracks(Barracks newBarracks)
    {
        barracks = newBarracks;
    }

    // ==========================================
    // RALLY POINT
    // ==========================================

    void GoToRallyPoint()
    {
        if (barracks == null ||
            !barracks.HasRallyPoint())
        {
            if (animator != null)
                animator.SetBool("Walk", false);

            return;
        }

        Vector3 rallyPosition =
            barracks.GetRallyPosition();

        float distance = Vector3.Distance(
            transform.position,
            rallyPosition
        );

        if (distance <= rallyStoppingDistance)
        {
            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            if (animator != null)
                animator.SetBool("Walk", false);

            return;
        }

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(rallyPosition);
        }

        if (animator != null)
            animator.SetBool("Walk", true);
    }

    // ==========================================
    // FIND NEAREST ENEMY
    // ==========================================

    void FindNearestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag(enemyTag);

        float closestDistance = Mathf.Infinity;
        EnemyCombat closestEnemy = null;

        foreach (GameObject enemyObject in enemies)
        {
            EnemyCombat enemy =
                enemyObject.GetComponent<EnemyCombat>();

            if (enemy == null)
                continue;

            if (enemy.IsDead())
                continue;

            if (enemy.IsEngaged())
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemyObject.transform.position
            );

            if (distance > detectionRange)
                continue;

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        currentEnemy = closestEnemy;

        if (currentEnemy != null)
        {
            currentEnemyHealth =
                currentEnemy.GetComponent<EnemyHealth>();

            if (agent.enabled && agent.isOnNavMesh)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    currentEnemy.transform.position
                );
            }

            if (animator != null)
                animator.SetBool("Walk", true);
        }
    }

    // ==========================================
    // START FIGHT
    // ==========================================

    void StartFight()
    {
        if (currentEnemy == null)
            return;

        if (currentEnemy.IsDead() ||
            currentEnemy.IsEngaged())
        {
            ClearEnemy();
            return;
        }

        engaged = true;

        StopForCombat();

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Attack", true);
        }

        currentEnemy.Engage(this);

        StartCoroutine(AttackRoutine());
    }

    // ==========================================
    // ATTACK ENEMY
    // ==========================================

    IEnumerator AttackRoutine()
    {
        while (engaged && currentEnemy != null)
        {
            yield return new WaitForSeconds(
                attackInterval
            );

            if (currentEnemy == null)
                break;

            if (currentEnemy.IsDead())
            {
                EnemyDied();
                yield break;
            }

            if (currentEnemyHealth != null)
            {
                currentEnemyHealth.TakeDamage(damage);

                if (currentEnemy == null ||
                    currentEnemy.IsDead())
                {
                    EnemyDied();
                    yield break;
                }
            }
        }
    }

    // ==========================================
    // ENEMY DIED
    // ==========================================

    public void EnemyDied()
    {
        if (isDead)
            return;

        engaged = false;

        currentEnemy = null;
        currentEnemyHealth = null;

        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        ResumeAfterCombat();
    }

    // ==========================================
    // CLEAR ENEMY
    // ==========================================

    void ClearEnemy()
    {
        currentEnemy = null;
        currentEnemyHealth = null;
        engaged = false;

        if (animator != null)
        {
            animator.SetBool("Attack", false);
        }

        if (!isDead)
        {
            ResumeAfterCombat();
        }
    }

    // ==========================================
    // ROTATE TOWARD ENEMY
    // ==========================================

    void RotateTowardsEnemy()
    {
        if (!engaged || currentEnemy == null)
            return;

        Transform objectToRotate =
            troopModel != null ? troopModel : transform;

        Vector3 direction =
            currentEnemy.transform.position -
            objectToRotate.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Model faces along positive X-axis
        Quaternion targetRotation =
            Quaternion.LookRotation(direction) *
            Quaternion.Euler(0f, -90f, 0f);

        objectToRotate.rotation = Quaternion.Slerp(
            objectToRotate.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    // ==========================================
    // DAMAGE
    // ==========================================

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    // ==========================================
    // BARRACKS DESTROYED
    // ==========================================

    public void BarracksDestroyed()
    {
        if (isDead)
            return;

        isDead = true;
        engaged = false;

        StopAllCoroutines();

        if (currentEnemy != null)
        {
            currentEnemy.TroopDied(this);
        }

        Destroy(gameObject);
    }

    // ==========================================
    // DIE
    // ==========================================

    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        engaged = false;

        StopAllCoroutines();

        if (currentEnemy != null)
        {
            currentEnemy.TroopDied(this);
        }

        if (barracks != null)
        {
            barracks.TroopDied(this);
        }

        Destroy(gameObject);
    }

    public bool IsEngaged()
    {
        return engaged;
    }
}
