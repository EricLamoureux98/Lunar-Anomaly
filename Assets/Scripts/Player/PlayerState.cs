using System;
using UnityEngine;
using LunarAnomaly.Gameplay;
using UnityEngine.InputSystem;
using LunarAnomaly.Input;

namespace LunarAnomaly.Player
{
    public class PlayerState : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] Airlock airlock;
        PlayerMovement playerMovement;
        PlayerInput playerInput;
        Oxygen oxygen;

        [SerializeField] Transform respawnPoint;
        [SerializeField] float oxygenGracePeriod = 3f;

        CurrentState currentState;
        float graceTimer;

        bool terminalProximity;     

        // To UIManager
        public static event Action<float> OnPlayerDying;
        // To TerminalUI and PlayerLook
        public static event Action<bool> OnTerminalUIActive;

        void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
            oxygen = GetComponent<Oxygen>();
        }

        void OnEnable()
        {
            Oxygen.OnOxygenDepleted += OnOxygenDepleted;
            TerminalController.OnTerminalProximity += TerminalProximity;
            InputHandler.OnInteractPressed += TerminalInteract;
            InputHandler.OnCloseUI += TryExitTerminal;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
            TerminalController.OnTerminalProximity -= TerminalProximity;
            InputHandler.OnInteractPressed -= TerminalInteract;
            InputHandler.OnCloseUI -= TryExitTerminal;
        }

        void Update()
        {
            switch (currentState)
            {
                case CurrentState.Suffocating:
                    HandleSuffocating();
                    break;
            }
        }

        void OnOxygenDepleted(bool isSuffocating)
        {
            if (isSuffocating)
                ChangeState(CurrentState.Suffocating);
            else
                ChangeState(CurrentState.Alive);
        }

        void HandleAlive()
        {
            // Probably not needed. Maybe use for UI
        }

        void HandleSuffocating()
        {
            graceTimer -= Time.deltaTime;

            if (graceTimer <= 0f)
            {
                ChangeState(CurrentState.Dead);
            }
        }

        void HandleDeath()
        {
            oxygen.SetActive(false);
            playerMovement.SetActive(false);
            airlock.ResetAirlock();
            
            Invoke("HandleRespawn", 1f); // <--- Make this a UI button eventually
        }

        void HandleRespawn()
        {
            if (respawnPoint == null) return;

            Rigidbody rb = GetComponent<Rigidbody>();
            
            // Reset velocity to prevent unexpected movement
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Move player
            rb.position = respawnPoint.position;
            rb.rotation = respawnPoint.rotation;

            oxygen.ResetOxygen();
            ChangeState(CurrentState.Alive);
        }

        void TerminalProximity(bool proximity)
        {
            terminalProximity = proximity;
        }
        
        void TerminalInteract()
        {
            if (currentState != CurrentState.Alive) return;

            TryEnterTerminal();
        }
        
        void TryEnterTerminal()
        {
            if (!terminalProximity) return;

            ChangeState(CurrentState.UsingTerminal);
        }

        void TryExitTerminal()
        {
            if (currentState == CurrentState.UsingTerminal)
            {
                ChangeState(CurrentState.Alive);
            }
        }

        void ChangeState(CurrentState newState)
        {
            if (newState == currentState) return;

            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
        }

        void EnterState(CurrentState state)
        {
            switch (state)
            {
                case CurrentState.Suffocating:
                    // Add visuals and sound 
                    OnPlayerDying?.Invoke(oxygenGracePeriod);
                    graceTimer = oxygenGracePeriod;
                    break;
                
                case CurrentState.Dead:
                    HandleDeath();
                    break;
                
                case CurrentState.UsingTerminal:
                    playerInput.SwitchCurrentActionMap("UI");
                    OnTerminalUIActive?.Invoke(true);
                    break;
            }
        }

        void ExitState(CurrentState state)
        {
            // This is for turning off things like audio and UI
            switch (state)
            {
                case CurrentState.Suffocating:
                    graceTimer = 0f;
                    // Stop suffocation SFX
                    // Hide oxygen warning UI
                    // Reset post-processing effects
                    break;
                
                case CurrentState.Dead:
                    playerMovement.SetActive(true);
                    oxygen.SetActive(true);
                    break;

                case CurrentState.UsingTerminal:
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    OnTerminalUIActive?.Invoke(false);
                    break;
            }
        }
    }

    public enum CurrentState
    {   Alive, 
        Suffocating, 
        Dead, 
        Respawning,
        UsingTerminal
    } 
}