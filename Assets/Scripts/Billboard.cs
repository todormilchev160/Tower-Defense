using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cam == null)
            return;

        // Point the object's negative Z axis toward the camera
        Vector3 direction = transform.position - cam.position;

        transform.rotation = Quaternion.LookRotation(direction);
    }
}