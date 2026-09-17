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

    private NavMeshAgent agent;

    private EnemyCombat currentEnemy;
    private EnemyHealth currentEnemyHealth;

    private bool engaged = false;
    private bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isDead)
            return;

        // Enemy was destroyed
        if (currentEnemy == null)
        {
            engaged = false;

            FindNearestEnemy();

            return;
        }

        if (engaged)
            return;

        float distance = Vector3.Distance(
            transform.position,
            currentEnemy.transform.position
        );

        // We are close enough to fight
        if (distance <= engagementDistance)
        {
            StartFight();
        }
        else
        {
            // Continue chasing enemy
            agent.isStopped = false;

            agent.SetDestination(
                currentEnemy.transform.position
            );
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

            // Don't select an enemy already fighting another troop
            if (enemy.IsEngaged())
                continue;

            float distance = Vector3.Distance(
                transform.position,
                enemyObject.transform.position
            );

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
        }
    }

    void StartFight()
    {
        if (currentEnemy == null)
            return;

        // Enemy may have become engaged with another troop
        if (currentEnemy.IsEngaged())
        {
            currentEnemy = null;
            currentEnemyHealth = null;
            return;
        }

        engaged = true;

        // Stop troop
        agent.isStopped = true;
        agent.ResetPath();

        // Stop enemy and make it attack us
        currentEnemy.Engage(this);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        while (engaged && currentEnemy != null)
        {
            yield return new WaitForSeconds(attackInterval);

            if (currentEnemyHealth != null)
            {
                currentEnemyHealth.TakeDamage(damage);
            }
        }

        // Enemy died
        engaged = false;
        currentEnemy = null;
        currentEnemyHealth = null;

        if (!isDead)
        {
            agent.isStopped = false;
        }
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

        // Tell enemy it won
        if (currentEnemy != null)
        {
            currentEnemy.TroopDied(this);
        }

        Destroy(gameObject);
    }
}