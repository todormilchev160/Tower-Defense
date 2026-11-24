using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static int currency=200;
    public static bool waveCleared=true;
    public TextMeshProUGUI currencyText;
    public GameObject waveSpawner;
    private int timespressed=0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      currencyText.text =currency+"";
    }
    public void StartWaves()
    {
        timespressed++;
        if(timespressed>1)
        {
            currency+=((int)WaveSpawner.clocktime/4);
            WaveSpawner.clocktime=0;
        }
        waveSpawner.SetActive(true);
    }
}
