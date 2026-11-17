using UnityEngine;
using UnityEngine.EventSystems;

public class ArrayHoverDeactivatio : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject[] objectsToActivate;


    void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(false);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }

    void OnDisable()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            obj.SetActive(true);
        }
    }
}