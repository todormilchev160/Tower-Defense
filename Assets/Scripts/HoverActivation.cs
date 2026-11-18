using UnityEngine;

public class HoverActivation : MonoBehaviour
{
    [SerializeField] private GameObject objectToActivate;

    public static bool selecting = false;

    private bool isLocked = false;

    public void SetHover(bool hovering)
    {
        Debug.Log($"SetHover({hovering}), selecting={selecting}, locked={isLocked}");
        // Ignore hover events while any menu is open
        if (selecting || isLocked)
            return;

        if (objectToActivate != null)
            objectToActivate.SetActive(hovering);
    }

    public void LockHover()
    {
        isLocked = true;

        // Keep the current slot visible
        if (objectToActivate != null)
            objectToActivate.SetActive(true);
    }

    public void UnlockHover()
    {
        isLocked = false;
    }
}