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
            UpdateCursorLock(false);
        }

        void OnEnable()
        {
            PlayerState.OnTerminalUIActive += UpdateCursorLock;
        }

        void OnDisable()
        {
            PlayerState.OnTerminalUIActive -= UpdateCursorLock;
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

        void UpdateCursorLock(bool unlocked)
        {
            if (!unlocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (unlocked)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            
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

        void ReadInput()
        {
            lookInput = inputHandler.CameraInput;
        }
    }
}
