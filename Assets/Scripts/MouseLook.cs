using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform cameraPivot; // Assigna aquí el "CameraPivot" (pare de la càmera)
    public Transform armsAndWeaponPivot;
    public float mouseSensibility = 150f;
    public float minVerticalAngle = -70f;
    public float maxVerticalAngle = 70f;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 mouseDelta = Vector2.zero;
        if (Mouse.current != null)
        {
            mouseDelta = Mouse.current.delta.ReadValue();
        }

        float mouseX = mouseDelta.x * mouseSensibility * Time.deltaTime;
        float mouseY = mouseDelta.y * mouseSensibility * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        // Rotem el PIVOT, deixant la rotació local de la càmera lliure per al Shake
        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (armsAndWeaponPivot != null)
        {
            armsAndWeaponPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // Rotació horitzontal del jugador
        transform.Rotate(Vector3.up * mouseX);
    }
}