using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static int currency=1000;
    public static bool waveCleared=true;
    public TextMeshProUGUI currencyText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      currencyText.text =currency+"$";
    }
}
