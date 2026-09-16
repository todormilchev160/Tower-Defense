using UnityEngine;
using UnityEngine.SceneManagement;

public class BaseHealth : MonoBehaviour
{
    [SerializeField]private float baseHealth=100;
    private EnemyDamageBase enemyDamageBase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(baseHealth);
    }
    void OnTriggerEnter(Collider other)
    {
        enemyDamageBase=other.GetComponent<EnemyDamageBase>();
        Debug.Log("Triggered");
        baseHealth-=enemyDamageBase.damageDealtOnBase;
        if(baseHealth<=0)
        {
            SceneManager.LoadScene("TodorScene");
        }
    }
}
