using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("Mouse")]
        [SerializeField] float sensitivity = 0.1f;
        [SerializeField] float maxSense = 0.6f;
        [SerializeField] float minSense = 0.05f;
        [SerializeField] float maxLookAngle = 80f;

        [Header("Debug")]
        public float Sensitivity => sensitivity;
        public float MaxSense => maxSense;
        public float MinSense => minSense;

        [SerializeField] Transform orientation;
        [SerializeField] Transform vision;
        InputHandler inputHandler;

        Vector2 lookInput;
        float xRotation;
        float currentSensitivity;

        bool cursorUnlocked;

        void OnEnable()
        {
            UIManager.OnCursorUnlock += UpdateCursorLock;
            OutpostUI.OnLogShown += UpdateCursorLock;
            OutpostRevealCinematic.OnSilhouetteSensitivity += CinematicSensitivity;
        }

        void OnDisable()
        {
            UIManager.OnCursorUnlock -= UpdateCursorLock;
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

        public void UpdateMouseSense(float sense)
        {
            sensitivity = sense;
            currentSensitivity = sensitivity;
        }

        void UpdateCursorLock(bool unlocked)
        {
            cursorUnlocked = unlocked;

            if (!cursorUnlocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (cursorUnlocked)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            
        }        

        void HandleLook()
        {
            if (cursorUnlocked) return;

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
                currentSensitivity = Sensitivity / 10f;
            else
                currentSensitivity = Sensitivity;
        }

        void ReadInput()
        {
            lookInput = inputHandler.CameraInput;
        }
    }
}
