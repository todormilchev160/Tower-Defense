
using UnityEngine;
using UnityEngine.InputSystem;

public class TowerHover : MonoBehaviour
{
    private GameObject assignedTowerSlot;

    public void AssignTowerSlot(GameObject slot)
    {
        assignedTowerSlot = slot;
    }

    void Update()
    {
        if (assignedTowerSlot == null)
            return;

        if (HoverActivation.selecting)
        {
            assignedTowerSlot.SetActive(false);
            return;
        }

        if (Camera.main == null || Mouse.current == null)
            return;

        Ray ray = Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()
        );

        bool hovering = false;

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            hovering =
                hit.transform == transform ||
                hit.transform.IsChildOf(transform);
        }

        assignedTowerSlot.SetActive(hovering);
    }

    void OnDisable()
    {
        if (assignedTowerSlot != null)
            assignedTowerSlot.SetActive(false);
    }
}
