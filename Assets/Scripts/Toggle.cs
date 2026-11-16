
using UnityEngine;
using UnityEngine.InputSystem;

public class Toggle : MonoBehaviour
{
    [Header("Toggle Settings")]
    [SerializeField] private Key toggleKey = Key.E;

    [SerializeField] private GameObject objectToToggle;

    void Update()
    {
        if (Keyboard.current == null || objectToToggle == null)
            return;

        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            objectToToggle.SetActive(!objectToToggle.activeSelf);
        }
    }
}
