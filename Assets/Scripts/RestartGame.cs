
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    [Header("Restart Settings")]
    [SerializeField] private Key restartKey = Key.R;

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current[restartKey].wasPressedThisFrame)
        {
            RestartScene();
        }
    }

    public void RestartScene()
    {
        // Reset static variables
        HoverActivation.selecting = false;
        GameManager.currency=200;
        GameManager.waveCleared=true;

        // Add other static variables here:
        // GameManager.currency = 100;
        // Barracks.spawningUnlocked = false;

        // Reload current scene
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
