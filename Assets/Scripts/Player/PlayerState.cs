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
        Rigidbody rb;

        [SerializeField] Transform respawnPoint;
        [SerializeField] float oxygenGracePeriod = 3f;

        PlayerCurrentState currentState;
        float graceTimer;

        bool terminalProximity;     

        // To UIManager
        public static event Action<float> OnPlayerDying;
        public static event Action<bool> OnHideGameplayUI;

        // To TerminalUI and PlayerLook
        public static event Action<bool> OnTerminalUIActive;

        // To AtmosphereTracker
        public static event Action OnResetPressure;

        void Awake()
        {
            rb = GetComponent<Rigidbody>();
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
            SanityManager.OnInsanity += HandleInsanity;
            OutpostController.OnLadderUsed += HandleTeleport;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
            TerminalController.OnTerminalProximity -= TerminalProximity;
            InputHandler.OnInteractPressed -= TerminalInteract;
            InputHandler.OnCloseUI -= TryExitTerminal;
            SanityManager.OnInsanity -= HandleInsanity;
            OutpostController.OnLadderUsed -= HandleTeleport;
        }

        void Update()
        {
            switch (currentState)
            {
                case PlayerCurrentState.Suffocating:
                    HandleSuffocating();
                    break;
            }
        }

        void OnOxygenDepleted(bool isSuffocating)
        {
            if (isSuffocating)
                ChangeState(PlayerCurrentState.Suffocating);
            else
                ChangeState(PlayerCurrentState.Alive);
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
                ChangeState(PlayerCurrentState.Dead);
            }
        }

        void HandleInsanity()
        {
            ChangeState(PlayerCurrentState.Insane);
        }

        void HandleDeath()
        {
            oxygen.SetActive(false);
            playerMovement.SetActive(false);
            airlock.ResetAirlock();
            
            Invoke("HandleRespawn", 1f); // <--- Make this a UI button eventually
        }

        void HandleGameOver()
        {
            // This is redundant 
            if (GameManager.Instance.CurrentState == GameState.GameOver) return; 

            GameManager.Instance.TriggerGameOver();
        }

        void HandleRespawn()
        {
            if (respawnPoint == null) return;
            
            // Reset velocity to prevent unexpected movement
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Move player
            rb.position = respawnPoint.position;
            rb.rotation = respawnPoint.rotation;

            OnResetPressure?.Invoke();
            oxygen.ResetOxygen();
            ChangeState(PlayerCurrentState.Alive);
        }

        // Can be called for any teleport need
        void HandleTeleport(Transform teleportPos)
        {
            if (currentState != PlayerCurrentState.Alive) return;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.position = teleportPos.position;
            rb.rotation = teleportPos.rotation;
        }

        void TerminalProximity(bool proximity)
        {
            terminalProximity = proximity;
        }
        
        void TerminalInteract()
        {
            if (currentState != PlayerCurrentState.Alive) return;

            TryEnterTerminal();
        }
        
        void TryEnterTerminal()
        {
            if (!terminalProximity) return;

            ChangeState(PlayerCurrentState.UsingTerminal);
        }

        void TryExitTerminal()
        {
            if (currentState == PlayerCurrentState.UsingTerminal)
            {
                ChangeState(PlayerCurrentState.Alive);
            }
        }

        void ChangeState(PlayerCurrentState newState)
        {
            if (newState == currentState) return;

            ExitState(currentState);
            currentState = newState;
            EnterState(newState);
        }

        void EnterState(PlayerCurrentState state)
        {
            switch (state)
            {
                case PlayerCurrentState.Suffocating:
                    // Add visuals and sound 
                    OnPlayerDying?.Invoke(oxygenGracePeriod);
                    graceTimer = oxygenGracePeriod;
                    break;
                
                case PlayerCurrentState.Dead:
                    HandleDeath();
                    break;

                case PlayerCurrentState.Insane:
                    HandleGameOver();
                    break;
                
                case PlayerCurrentState.UsingTerminal:
                    playerInput.SwitchCurrentActionMap("UI");
                    OnHideGameplayUI?.Invoke(true);
                    OnTerminalUIActive?.Invoke(true);
                    break;
            }
        }

        void ExitState(PlayerCurrentState state)
        {
            // This is for turning off things like audio and UI
            switch (state)
            {
                case PlayerCurrentState.Suffocating:
                    graceTimer = 0f;
                    // Stop suffocation SFX
                    // Hide oxygen warning UI
                    // Reset post-processing effects
                    break;
                
                case PlayerCurrentState.Dead:
                    playerMovement.SetActive(true);
                    oxygen.SetActive(true);
                    break;

                case PlayerCurrentState.UsingTerminal:
                    playerInput.SwitchCurrentActionMap("Gameplay");
                    OnTerminalUIActive?.Invoke(false);
                    OnHideGameplayUI?.Invoke(false);
                    break;
            }
        }
    }

    public enum PlayerCurrentState
    {   Alive, 
        Suffocating, 
        Dead, 
        Respawning,
        Insane,
        UsingTerminal
    } 
}