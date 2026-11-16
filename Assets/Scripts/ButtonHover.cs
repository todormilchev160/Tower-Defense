
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject objectToActivate;
   

    void Start()
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover");
      
        if (objectToActivate != null)
            objectToActivate.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }

    void OnDisable()
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(false);
    }
}
