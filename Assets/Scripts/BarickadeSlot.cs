using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
public class BarickadeSlot : MonoBehaviour
{
    public GameObject barickadePrefab;
    public float spawnHeight;
    public int barickadePrice;
    public GameObject slotButton;
    public TextMeshProUGUI slotText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotText.text="Barrickade"+barickadePrice;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnBarickade()
    {
       if(GameManager.currency<barickadePrice)
       return;
       GameManager.currency-=barickadePrice;
           Vector3 spawnPosition=new Vector3(transform.position.x,spawnHeight,transform.position.z);
        Instantiate(barickadePrefab,spawnPosition,transform.rotation,transform); 
        slotButton.SetActive(false);
    }
}
