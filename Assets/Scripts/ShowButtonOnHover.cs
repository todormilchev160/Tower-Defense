using UnityEngine;
using UnityEngine.EventSystems;

public class ShowButtonOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject buttonToShow;

    void Start()
    {
        buttonToShow.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonToShow.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonToShow.SetActive(false);
    }
}