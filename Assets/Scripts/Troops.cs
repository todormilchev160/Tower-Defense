using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Troops : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float health = 20f;

    [Header("Combat")]
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackInterval = 1f;

    [Tooltip("Distance at which the troop and enemy stop and start fighting")]
    [SerializeField] private float engagementDistance = 2f;

    [Header("Targeting")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private float detectionRange = 10f;

    private NavMeshAgent agent;

    private EnemyCombat currentEnemy;
    private EnemyHealth currentEnemyHealth;

    // IMPORTANT: NOT STATIC
    private bool engaged = false;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDead || engaged)
            return;

        // If we don't currently have an enemy, find one
        if (currentEnemy == null)
        {
            FindNearestEnemy();

            if (currentEnemy == null)
                return;
        }

        // Enemy may have died while we were moving toward it
        if (currentEnemy.IsDead())
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
            agent.isStopped = false;
            agent.SetDestination(currentEnemy.transform.position);
        }
    }

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

            // IMPORTANT:
            // continue, NOT return
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
            agent.SetDestination(currentEnemy.transform.position);
        }
    }

    void StartFight()
    {
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

    IEnumerator AttackRoutine()
    {
        while (engaged && currentEnemy != null)
        {
            yield return new WaitForSeconds(attackInterval);

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

                // Check immediately after damaging it
                if (currentEnemy == null || currentEnemy.IsDead())
                {
                    EnemyDied();
                    yield break;
                }
            }
        }
    }

    public void EnemyDied()
    {
        if (isDead)
            return;

        Debug.Log("Enemy killed - looking for next enemy");

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

        Destroy(gameObject);
    }

    public bool IsEngaged()
    {
        return engaged;
    }
}