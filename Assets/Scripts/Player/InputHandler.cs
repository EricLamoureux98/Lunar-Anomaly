using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LunarAnomaly.Input
{
    public class InputHandler : MonoBehaviour
    {
        public Vector2 CameraInput { get; private set; }
        public Vector2 MoveInput { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool SprintHeld { get; private set; }
        public bool UseToolHeld { get; private set; }

        public static event Action OnInteractPressed;
        public static event Action OnCloseUI;

        public void Look(InputAction.CallbackContext context)
        {
            CameraInput = context.ReadValue<Vector2>();
        }

        public void Move(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
        }

        public void Jump(InputAction.CallbackContext context)
        {
            if (context.performed)
                JumpHeld = true;

            if (context.canceled)
                JumpHeld = false;
        }

        public void Sprint(InputAction.CallbackContext context)
        {
            if (context.performed)
                SprintHeld = true;

            if (context.canceled)
                SprintHeld = false;
        }

        public void UseTool(InputAction.CallbackContext context)
        {
            if (context.performed)
                UseToolHeld = true;

            if (context.canceled)
                UseToolHeld = false;
        }

        public void SystemInteract(InputAction.CallbackContext context)
        {
            OnInteractPressed?.Invoke();
        }

        //----------------------------UI---------------------------------//

        public void InteractWithUI(InputAction.CallbackContext context)
        {
            
        }

        public void ExitUI(InputAction.CallbackContext context)
        {
            OnCloseUI?.Invoke();
        }
    }
}
