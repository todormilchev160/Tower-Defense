using UnityEngine;

public class HoverActivation : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;
    public static bool selecting;

    public void SetHover(bool hovering)
    {
        if(selecting)
        return;
        if (objectToActivate != null)
            objectToActivate.SetActive(hovering);
    }
}