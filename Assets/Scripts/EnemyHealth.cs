using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
   [SerializeField]private float health=10;
   [SerializeField]private bool isLastEnemy;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TakeDamage(float damage)
    {
        health-=damage;
        if (health <= 0)
        {
           Die();
        }
    }
    void Die()
    {
        if(isLastEnemy)
        {
            GameManager.waveCleared=true;
        }
        Destroy(gameObject);
    }
}
