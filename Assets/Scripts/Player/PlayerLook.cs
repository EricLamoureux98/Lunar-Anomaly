using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] Transform orientation;
        [SerializeField] Transform vision;
        [SerializeField] float sensitivity = 0.1f;
        [SerializeField] float maxLookAngle = 80f;
        InputHandler inputHandler;

        Vector2 lookInput;
        float xRotation;
        float currentSensitivity;

        void OnEnable()
        {
            PlayerState.OnTerminalUIActive += UpdateCursorLock;
            OutpostUI.OnLogShown += UpdateCursorLock;
            OutpostRevealCinematic.OnSilhouetteSensitivity += CinematicSensitivity;
        }

        void OnDisable()
        {
            PlayerState.OnTerminalUIActive -= UpdateCursorLock;
            OutpostUI.OnLogShown -= UpdateCursorLock;
            OutpostRevealCinematic.OnSilhouetteSensitivity -= CinematicSensitivity;
        }

        void Awake()
        {
            inputHandler = GetComponent<InputHandler>();
            if (inputHandler == null) Debug.Log("PlayerInput not found!");
        }

        void Start()
        {
            UpdateCursorLock(false);
            currentSensitivity = sensitivity;
            //Screen.SetResolution(860, 520, true);
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
            float mouseX = lookInput.x * currentSensitivity;
            float mouseY = lookInput.y * currentSensitivity;            

            orientation.Rotate(Vector3.up * mouseX);

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
            vision.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        void CinematicSensitivity(bool enabled)
        {
            if (enabled)
                currentSensitivity = sensitivity / 10f;
            else
                currentSensitivity = sensitivity;
        }

        void ReadInput()
        {
            lookInput = inputHandler.CameraInput;
        }
    }
}
