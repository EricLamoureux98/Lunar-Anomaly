using System;
using UnityEngine;
using LunarAnomaly.Gameplay;
using System.Collections;

namespace LunarAnomaly.Player
{
    public class PlayerState : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] HabitatAirlock airlock;
        PlayerMovement playerMovement;
        Oxygen oxygen;
        Rigidbody rb;

        [SerializeField] Transform respawnPoint;
        [SerializeField] float oxygenGracePeriod = 3f;
        [SerializeField] bool respawnInHabitat;

        [Header("Debug")]
        public bool RespawnInHabitat => respawnInHabitat;
        [SerializeField] Transform teleportPoint;

        PlayerCurrentState currentState;
        public PlayerCurrentState CurrentState => currentState;
          
        float graceTimer;

        Coroutine breathingRoutine;

        // To ScreenEffect
        public static event Action OnPlayerDying;
        // public static event Action<bool> OnHideGameplayUI; 
        public static event Action OnLadderTeleport;

        // To AtmosphereTracker
        public static event Action OnResetPressure;

        void Awake()
        {
            if (GameManager.Instance)
                GameManager.Instance.RegisterPlayer(gameObject);
                
            oxygen = GetComponent<Oxygen>();            
            
            rb = GetComponent<Rigidbody>();
            playerMovement = GetComponent<PlayerMovement>();
        }

        void OnEnable()
        {
            Oxygen.OnOxygenDepleted += OnOxygenDepleted;
            SanityManager.OnInsanity += HandleInsanity;
            OutpostController.OnLadderUsed += HandleLadderTeleport;
            OutpostRevealCinematic.OnOutpostCinematicTeleport += RequestTeleport;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenDepleted -= OnOxygenDepleted;
            SanityManager.OnInsanity -= HandleInsanity;
            OutpostController.OnLadderUsed -= HandleLadderTeleport;
            OutpostRevealCinematic.OnOutpostCinematicTeleport -= RequestTeleport;
        }

        void Start()
        {
            EnterState(PlayerCurrentState.Alive);

            if (respawnInHabitat) HandleRespawn();
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
            HandleRespawn();
        }

        void HandleGameOver()
        {
            // This is redundant 
            if (GameManager.Instance.CurrentState == GameState.GameOver) return; 

            GameManager.Instance.TriggerGameOver();
        }

        public void UpdateRespawnInHabitat(bool inHabitat)
        {
            respawnInHabitat = inHabitat;
        }

        public void HandleDebugTeleport()
        {
            RequestTeleport(teleportPoint, TeleportType.Cinematic);
        }

        // Public for debug menu
        public void HandleRespawn()
        {
            if (respawnPoint == null) return;
            
            RequestTeleport(respawnPoint, TeleportType.Respawn);
            ChangeState(PlayerCurrentState.Alive);
        }

        void HandleLadderTeleport(Transform ladderTeleportPos)
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
                    fadeTime = 0f;
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
                oxygen.ResetOxygen();
            }

            if (type == TeleportType.Cinematic)
            {
                playerMovement.SetActive(true);
            }

            // Make sure player TP before resetting pressure
            yield return new WaitForSeconds(0.5f);

            OnResetPressure?.Invoke();
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
            }
        }
    }

    public enum PlayerCurrentState
    {   Alive, 
        Suffocating, 
        Dead, 
        Respawning,
        Insane,
    } 

    public enum TeleportType
    {
        Respawn,
        Ladder,
        Cinematic
    }
}