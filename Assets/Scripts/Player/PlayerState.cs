using System;
using UnityEngine;
using LunarAnomaly.Gameplay;
using UnityEngine.InputSystem;
using LunarAnomaly.Input;
using System.Collections;

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

        [SerializeField] bool testRespawnPlayer;

        PlayerCurrentState currentState;
        bool terminalProximity;     
        float graceTimer;

        Coroutine breathingRoutine;

        // To UIManager
        public static event Action OnPlayerDying;
        public static event Action<bool> OnHideGameplayUI; 
        public static event Action OnLadderTeleport;

        // To TerminalUI and PlayerLook
        public static event Action<bool> OnTerminalUIActive;

        // To AtmosphereTracker
        public static event Action OnResetPressure;

        void Awake()
        {
            if (GameManager.Instance)
                GameManager.Instance.RegisterPlayer(gameObject);
                
            oxygen = GetComponent<Oxygen>();

            
            
            rb = GetComponent<Rigidbody>();
            playerInput = GetComponent<PlayerInput>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void OnEnable()
        {
            Oxygen.OnOxygenDepleted += OnOxygenDepleted;
            TerminalController.OnTerminalProximity += TerminalProximity;
            InputHandler.OnInteractPressed += TerminalInteract;
            InputHandler.OnCloseUI += TryExitTerminal;
            SanityManager.OnInsanity += HandleInsanity;
            OutpostController.OnLadderUsed += HandleLadder;
            OutpostRevealCinematic.OnOutpostCinematicTeleport += RequestTeleport;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
            TerminalController.OnTerminalProximity -= TerminalProximity;
            InputHandler.OnInteractPressed -= TerminalInteract;
            InputHandler.OnCloseUI -= TryExitTerminal;
            SanityManager.OnInsanity -= HandleInsanity;
            OutpostController.OnLadderUsed -= HandleLadder;
            OutpostRevealCinematic.OnOutpostCinematicTeleport -= RequestTeleport;
        }

        void Start()
        {
            EnterState(PlayerCurrentState.Alive);
        }

        void Update()
        {
            switch (currentState)
            {
                case PlayerCurrentState.Suffocating:
                    HandleSuffocating();
                    break;
            }

            if (testRespawnPlayer)
            {
                HandleRespawn();
                testRespawnPlayer = false;
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
            //SoundManager.PlaySound(SoundType.Breathing, 0.5f, false);

            breathingRoutine = StartCoroutine(BreathingRoutine());
        }

        IEnumerator BreathingRoutine()
        {
            while (true)
            {
                yield return new WaitUntil(() => oxygen.oxygenDraining);

                float waitTime = UnityEngine.Random.Range(4.5f, 7f);
                yield return new WaitForSeconds(waitTime);

                if (!oxygen.oxygenDraining) continue;

                SoundManager.PlaySound(SoundType.Breathing, 0.5f, false);
            }
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
            
            RequestTeleport(respawnPoint, TeleportType.Respawn);
            ChangeState(PlayerCurrentState.Alive);
        }

        void HandleLadder(Transform ladderTeleportPos)
        {
            if (currentState != PlayerCurrentState.Alive) return;

            OnLadderTeleport?.Invoke();
            RequestTeleport(ladderTeleportPos, TeleportType.Ladder);
        }

        void RequestTeleport(Transform destination, TeleportType type)
        {
            StartCoroutine(TeleportRoutine(destination, type));
        }

        IEnumerator TeleportRoutine(Transform destination, TeleportType type)
        {
            float fadeTime = 0f;
            bool resetOxygen = false;

            switch(type)
            {
                case TeleportType.Respawn:
                    fadeTime = 2f;
                    resetOxygen = true;
                    break;

                case TeleportType.Ladder:
                    fadeTime = 0.25f;
                    break;

                case TeleportType.Cinematic:
                    fadeTime = 0.1f;
                    playerMovement.SetActive(false);
                    break;
            }

            yield return new WaitForSecondsRealtime(fadeTime);

            // Reset velocity to prevent unexpected movement
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Move player
            rb.position = destination.position;
            rb.rotation = destination.rotation;

            if (resetOxygen)
            {
                OnResetPressure?.Invoke();
                oxygen.ResetOxygen();
            }

            if (type == TeleportType.Cinematic)
            {
                playerMovement.SetActive(true);
            }
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
                case PlayerCurrentState.Alive:
                    HandleAlive();                   
                    break;

                case PlayerCurrentState.Suffocating:
                    // Add visuals and sound 
                    OnPlayerDying?.Invoke();
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
                case PlayerCurrentState.Alive:
                    if (breathingRoutine != null)
                    {
                        StopCoroutine(breathingRoutine);
                        breathingRoutine = null;
                    }
                    break;

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

    public enum TeleportType
    {
        Respawn,
        Ladder,
        Cinematic
    }
}