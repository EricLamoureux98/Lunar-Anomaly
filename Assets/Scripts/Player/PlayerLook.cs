using LunarAnomaly.Input;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] Transform orientation;
        [SerializeField] Transform vision;
        [SerializeField] float sensistivity = 100f;
        [SerializeField] float maxLookAngle = 80f;
        InputHandler inputHandler;

        Vector2 lookInput;
        float xRotation;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler == null) Debug.Log("PlayerInput not found!");
        }

        void Update()
        {
            ReadInput();   
            HandleLook();
        }

        void ReadInput()
        {
            lookInput = inputHandler.CameraInput;
        }

        void HandleLook()
        {
            Vector2 look = lookInput;

            float mouseX = look.x * sensistivity * Time.deltaTime;
            float mouseY = look.y * sensistivity * Time.deltaTime;

            orientation.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
            vision.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
