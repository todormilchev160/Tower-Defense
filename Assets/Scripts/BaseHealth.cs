using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseHealth : MonoBehaviour
{
    private float baseHealth;
    public int amountOfHealthIncreasedOnUpgrade;
    [Header("Prices")]
    public int baseUgradePrice=100;
    public int upgradePriceIncreaseAfterUpgrade=50;
    public int moneyAfterDowngrade;
    [SerializeField]private float maxHealth=100;
    private EnemyDamageBase enemyDamageBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        baseHealth=maxHealth;
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(baseHealth);
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
          enemyDamageBase=other.GetComponent<EnemyDamageBase>();
          baseHealth-=enemyDamageBase.damageDealtOnBase;
          Destroy(other.gameObject);
        }
        enemyDamageBase=other.GetComponent<EnemyDamageBase>();
        baseHealth-=enemyDamageBase.damageDealtOnBase;
        if(baseHealth<=0)
        {
            SceneManager.LoadScene("TodorScene");
        }
    }
    public void UpgradeBase()
    {
        if(GameManager.currency<baseUgradePrice)
        {
            return;
        }
        else
        {
            maxHealth+=amountOfHealthIncreasedOnUpgrade;
            baseHealth+=amountOfHealthIncreasedOnUpgrade;
            GameManager.currency-=baseUgradePrice;
        }
    }

}
