using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Troops : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 20f;
    private float health;
    [SerializeField] private Image healthbarfill;

    [Header("Combat")]
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackInterval = 1f;

    [Tooltip("Distance at which the troop and enemy stop and start fighting")]
    [SerializeField] private float engagementDistance = 2f;

    [Header("Targeting")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float detectionRange = 10f;

    [Header("Rally Point")]
    [Tooltip("How close the troop needs to get to the rally point")]
    [SerializeField] private float rallyStoppingDistance = 0.5f;

    private NavMeshAgent agent;

    private EnemyCombat currentEnemy;
    private EnemyHealth currentEnemyHealth;
    private Animator animator;

    private bool engaged = false;
    private bool isDead = false;
    private Barracks barracks;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        health = maxHealth;
    }


    void Update()
    {
        healthbarfill.fillAmount = health / maxHealth;

        if (isDead || engaged)
            return;

        if (currentEnemy == null)
        {
            FindNearestEnemy();

            if (currentEnemy == null)
            {
                GoToRallyPoint();
                return;
            }
        }


        // ==========================================
        // CURRENT ENEMY DIED
        // ==========================================

        if (currentEnemy.IsDead())
        {
            ClearEnemy();
            return;
        }


        // ==========================================
        // CHASE / FIGHT ENEMY
        // ==========================================

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
            agent.isStopped = false;

            agent.SetDestination(
                currentEnemy.transform.position
            );

            animator.SetBool("Walk", true);
        }
    }


    // =====================================================
    // RALLY POINT
    // =====================================================

    void GoToRallyPoint()
    {
        if (!TroopRallyPoint.HasRallyPoint)
        {
            animator.SetBool("Walk", false);
            return;
        }

        float distance = Vector3.Distance(
            transform.position,
            TroopRallyPoint.RallyPosition
        );

        if (distance <= rallyStoppingDistance)
        {
            agent.isStopped = true;
            agent.ResetPath();

            animator.SetBool("Walk", false);

            return;
        }

        agent.isStopped = false;

        agent.SetDestination(
            TroopRallyPoint.RallyPosition
        );

        animator.SetBool("Walk", true);
    }


    // =====================================================
    // FIND ENEMY
    // =====================================================

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

            agent.isStopped = false;

            agent.SetDestination(
                currentEnemy.transform.position
            );

            animator.SetBool("Walk", true);
        }
    }


    // =====================================================
    // START FIGHT
    // =====================================================

    void StartFight()
    {
        animator.SetBool("Walk", false);
        animator.SetBool("Attack", true);

        if (currentEnemy == null)
            return;

        if (currentEnemy.IsDead())
        {
            ClearEnemy();
            return;
        }

        if (currentEnemy.IsEngaged())
        {
            ClearEnemy();
            return;
        }

        agent.isStopped = true;
        agent.ResetPath();

        engaged = true;

        currentEnemy.Engage(this);

        StartCoroutine(AttackRoutine());
    }


    // =====================================================
    // ATTACK
    // =====================================================

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


    // =====================================================
    // ENEMY DIED
    // =====================================================

    public void EnemyDied()
    {
        if (isDead)
            return;

        animator.SetBool("Attack", false);

        engaged = false;
        currentEnemy = null;
        currentEnemyHealth = null;

        agent.isStopped = false;
    }




    void ClearEnemy()
    {
        currentEnemy = null;
        currentEnemyHealth = null;
        engaged = false;

        animator.SetBool("Attack", false);

        if (!isDead)
            agent.isStopped = false;
    }

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
        barracks.TroopDied();
        Destroy(gameObject);
    }
    public void SetBarracks(Barracks newBarracks)
{
    barracks = newBarracks;
}

    public bool IsEngaged()
    {
        return engaged;
    }
}