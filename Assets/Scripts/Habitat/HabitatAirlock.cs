
using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class HabitatAirlock : MonoBehaviour
    {
        [SerializeField] CinemachineImpulseSource habitatImpulseSource;
        [SerializeField] Animator animExt;
        [SerializeField] Animator animInt;
        [SerializeField] Animator animLighting;
        [SerializeField] Transform smokeSpawnR, smokeSpawnL;
        [SerializeField] ParticleSystem smokeParticle;
        [SerializeField] float pressurizationTime = 3f;
        [SerializeField] float cooldownTime = 2f;
        [SerializeField] float maxWaitTime = 10f;
        AtmosphereZone atmosphereZone; // Might not need to be serialized
        float lastCycleTime = float.NegativeInfinity;
        float startCycleTime = float.NegativeInfinity;

        [SerializeField] Collider exteriorDoorCollider;
        [SerializeField] Collider interiorDoorCollider;

        bool isCycling = false;
        bool playerInside = false;
        bool cancelCycle; // Use this if/when the player is killed or level reset etc

        [SerializeField] bool testEnterFromExterior = false;
        [SerializeField] bool testEnterFromInterior = false;

        // To UIManager
        public static Action<bool> OnEnterAtmosphere;
        // To TerminalNotification
        public static event Action OnAirlockCycled;

        void Awake()
        {
            atmosphereZone = GetComponentInChildren<AtmosphereZone>();
            if (atmosphereZone == null) Debug.Log("Atmosphere zone not found");
        }

        void OnEnable()
        {
            HabitatController.OnEnterHabitat += EnterFromExterior;
            HabitatController.OnExitHabitat += EnterFromInterior;
        }

        void OnDisable()
        {
            HabitatController.OnEnterHabitat -= EnterFromExterior;
            HabitatController.OnExitHabitat -= EnterFromInterior;
        }

        void Update()
        {
            AirlockTesting();

            if (isCycling)
                AirlockWaitLimit();
            
            if (playerInside && !isCycling)
                TryCycle(true);
        }

        void EnterFromExterior() => TryCycle(fromExterior: true);
        void EnterFromInterior() => TryCycle(fromExterior: false);

        void TryCycle(bool fromExterior)
        {
            if (isCycling) return;
            if (Time.time - lastCycleTime < cooldownTime) return;
            StartCoroutine(CycleAirlock(fromExterior));
        }

        IEnumerator CycleAirlock(bool fromExterior)
        {
            isCycling = true;
            cancelCycle = false;

            // Make sure inside pressure is always right
            atmosphereZone.SetPressuized(!fromExterior);

            startCycleTime = Time.time;

            // Which animator is the entry/exit side
            Animator entryAnim = fromExterior ? animExt : animInt;
            Animator exitAnim = fromExterior ? animInt : animExt;

            // Ensure exit is closed
            exitAnim.SetBool("IsOpen", false);
            yield return new WaitForSeconds(1f);

            // Open entry side
            entryAnim.SetBool("IsOpen", true);

            if (!fromExterior)
                {
                    SoundManager.PlaySound(SoundType.Airlock, 0.15f);
                    yield return new WaitForSeconds(1.1f);
                }

            SoundManager.PlaySound(SoundType.DoorOpen, 1.5f);
            yield return new WaitForSeconds(1f);

            // Wait for player to enter (or cancel)
            AirlockWaitLimit();
            yield return new WaitUntil(() => playerInside || cancelCycle);
            yield return new WaitForSeconds(0.5f);
            // ---- animLighting.SetBool("isActive", true);

            if (cancelCycle)
            {
                ResetAirlock();
                yield break;
            }

            // Close entry side and pressurize
            entryAnim.SetBool("IsOpen", false);
            yield return new WaitForSeconds(1f);

            SoundManager.PlaySound(SoundType.DoorClose, 1.65f);
            CameraShakeManager.Instance.CameraShake(habitatImpulseSource, 0.04f);

            yield return new WaitForSeconds(1f);
            SoundManager.PlaySound(SoundType.Alarm, 1.25f);

            yield return new WaitForSeconds(1f);
            SoundManager.PlaySound(fromExterior ? SoundType.GainAtmosphere : SoundType.LoseAtmosphere, 1f);
            if (fromExterior && smokeParticle != null)
            {
                Instantiate(smokeParticle, smokeSpawnL.position, smokeSpawnL.rotation);
                Instantiate(smokeParticle, smokeSpawnR.position, smokeSpawnR.rotation);
            }

            yield return new WaitForSeconds(pressurizationTime);

            // Atmosphere state flips depending on direction
            atmosphereZone.SetPressuized(fromExterior);
            OnEnterAtmosphere?.Invoke(fromExterior);

            // Open exit side
            // ---- animLighting.SetBool("isActive", false);
            exitAnim.SetBool("IsOpen", true);
            SoundManager.PlaySound(SoundType.DoorOpen, 0.5f);

            // Wait for player to leave, then close exit door
            yield return new WaitUntil(() => !playerInside || cancelCycle);
            yield return new WaitForSeconds(1f);
            exitAnim.SetBool("IsOpen", false);
            SoundManager.PlaySound(SoundType.DoorOpen, 0.5f);
            OnAirlockCycled?.Invoke();

            lastCycleTime = Time.time;
            isCycling = false;
        }

        void AirlockWaitLimit()
        {
            if (!isCycling) return;
            if (playerInside) return;
            
            if (Time.time - startCycleTime >= maxWaitTime)
                cancelCycle = true;
        }

        public void PlayerInsideAirlock()
        {
            playerInside = true;
        }

        public void PlayerExitedAirlock()
        {
            playerInside = false;
        }

        public void ResetAirlock()
        {
            animExt.SetBool("IsOpen", false);
            animInt.SetBool("IsOpen", false);
            // animLighting.SetBool("isActive", false);

            isCycling = false;
            playerInside = false;
            cancelCycle = false;
        }

        public void ExternalDoorColliderActive(bool active)
        {
            exteriorDoorCollider.enabled = active;
        }

        public void InternalDoorColliderActive(bool active)
        {
            interiorDoorCollider.enabled = active;
        }

        void AirlockTesting()
        {
            if (testEnterFromExterior)
            {
                testEnterFromExterior = false;
                EnterFromExterior();
            }

            if (testEnterFromInterior)
            {
                testEnterFromInterior = false;
                EnterFromInterior();
            }
        }
    }
}