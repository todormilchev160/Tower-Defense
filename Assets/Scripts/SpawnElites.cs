using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class SpawnElites : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject elitePrefab;
    public int spawnAmount = 3;
    public float spawnSpacing = 1.5f;

    [Header("NavMesh")]
    public float navMeshSearchDistance = 5f;
    [Header("Prices")]
    public int elitesPrice;
    public TextMeshProUGUI priceText;
    public float cooldown;
    public Image fillImage;
    private float cooldown1;
    public GameObject glow;
    
    void Start()
    {
        cooldown1=cooldown;
    }
    void Update()
    {
        if(cooldown1>=cooldown)
        {
            glow.SetActive(true);
        }
        priceText.text=""+ elitesPrice;
        cooldown1++;
        fillImage.fillAmount=cooldown1/cooldown;
    }

    public void SpawnTroops()
    {
        if(cooldown1<=cooldown)
            return;
        
        if(GameManager.currency<elitesPrice)
        return;
         GameManager.currency-=elitesPrice;
          glow.SetActive(false);
         cooldown1=0;
        for (int i = 0; i < spawnAmount; i++)
        {
          
            Vector3 offset = new Vector3(
                (i - (spawnAmount - 1) / 2f) * spawnSpacing,
                0f,
                0f
            );

            Vector3 desiredPosition = transform.position + offset;

            // Find nearest valid NavMesh position
            if (NavMesh.SamplePosition(
                desiredPosition,
                out NavMeshHit hit,
                navMeshSearchDistance,
                NavMesh.AllAreas))
            {
                Instantiate(
                    elitePrefab,
                    hit.position,
                    transform.rotation
                );
            }
            else
            {
                Debug.LogWarning("No NavMesh found near spawn position!");
            }
        }
    }
}
