using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 10;
    private float health;
    [SerializeField] private bool isLastEnemy;
    [SerializeField] private int amountOfMoney;
    public Image healthbarFill;
    public TextMeshProUGUI damageText;
    public float damagefeedbacktime=1;

    private bool isDead = false;
    void Start()
    {
        health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;
        StartCoroutine(DamageFeedback(damage));

        if (health <= 0)
        {
            Die();
        }
    }
    IEnumerator DamageFeedback(float damage)
    {
        damageText.text="-"+damage;
        yield return new WaitForSeconds(damagefeedbacktime);
        damageText.text="";
    }

    void Die()
    {
        GameManager.currency+=amountOfMoney;
        
        isDead = true;

        if (isLastEnemy)
        {
            GameManager.waveCleared = true;
        }

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
     void Update()
    {
        healthbarFill.fillAmount =health/maxHealth;
    }
}