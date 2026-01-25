using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField] Transform orientation;
    [SerializeField] Transform vision;
    [SerializeField] float sensistivity = 100f;
    [SerializeField] float maxLookAngle = 80f;

    float xRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Look(InputAction.CallbackContext context)
    {
        Vector2 look = context.ReadValue<Vector2>();

        float mouseX = look.x * sensistivity * Time.deltaTime;
        float mouseY = look.y * sensistivity * Time.deltaTime;

        orientation.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        vision.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
