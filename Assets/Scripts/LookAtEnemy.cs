using UnityEngine;

public class TowerLookAtEnemy : MonoBehaviour
{
    [Header("Targeting")]
    public Transform targetPoint;
    public string enemyTag = "Enemy";

    [Header("Rotation")]
    public float rotationSpeed = 5f;

    private Transform currentTarget;

    void Update()
    {
        FindClosestEnemy();

        if (currentTarget != null)
        {
            RotateTowardsTarget();
        }
    }

    void FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        float closestDistanceToPoint = Mathf.Infinity;
        Transform closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            // Check if enemy is within this tower's range
            float distanceFromTower = Vector3.Distance(
                transform.position,
                enemy.transform.position
            );


            // Prioritize enemy closest to targetPoint
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

    void RotateTowardsTarget()
    {
        Vector3 direction = currentTarget.position - transform.position;

        // Don't tilt up/down
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}