using System.Collections;
using UnityEngine;

public class StartTutorial : MonoBehaviour
{
    public GameObject tutorialtext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DisplayCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator DisplayCoroutine()
    {
        tutorialtext.SetActive(true);
        yield return new WaitForSeconds(2);
        tutorialtext.SetActive(false);
    }
}
