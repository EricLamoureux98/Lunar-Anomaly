
using System;
using System.Collections;

using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class Airlock : MonoBehaviour
    {
        [SerializeField] Animator animExt;
        [SerializeField] Animator animInt;
        [SerializeField] Animator animLighting;
        [SerializeField] Transform smokeSpawnR, smokeSpawnL;
        [SerializeField] ParticleSystem smokeParticle;
        [SerializeField] AtmosphereZone atmosphereZone; // Might not need to be serialized
        [SerializeField] float pressurizationTime = 3f;
        [SerializeField] float cooldownTime = 2f;
        float lastCycleTime = float.NegativeInfinity;

        bool isCycling = false;
        bool playerInside = false;
        bool cancelCycle; // Use this if/when the player is killed or level reset etc

        [SerializeField] bool testEnterFromExterior = false;
        [SerializeField] bool testEnterFromInterior = false;

        // To UIManager
        public static Action<bool> OnEnterAtmosphere;

        void Awake()
        {
            atmosphereZone = GetComponentInChildren<AtmosphereZone>();
            if (atmosphereZone == null) Debug.Log("Atmosphere zone not found");
        }

        void Update()
        {
            AirlockTesting();
        }

        public void EnterFromExterior() => TryCycle(fromExterior: true);
        public void EnterFromInterior() => TryCycle(fromExterior: false);

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

            // Which animator is the entry/exit side
            Animator entryAnim = fromExterior ? animExt : animInt;
            Animator exitAnim = fromExterior ? animInt : animExt;

            // Ensure exit is closed
            exitAnim.SetBool("IsOpen", false);
            yield return new WaitForSeconds(1f);

            // Open entry side
            entryAnim.SetBool("IsOpen", true);
            SoundManager.PlaySound(SoundType.Airlock, 0.5f);
            yield return new WaitForSeconds(2f);

            // Wait for player to enter (or cancel)
            yield return new WaitUntil(() => playerInside || cancelCycle);
            yield return new WaitForSeconds(0.5f);
            animLighting.SetBool("isActive", true);
            SoundManager.PlaySound(SoundType.Alarm, 1.25f, false);

            if (cancelCycle)
            {
                ResetAirlock();
                yield break;
            }

            // Close entry side and pressurize
            entryAnim.SetBool("IsOpen", false);
            SoundManager.PlaySound(fromExterior ? SoundType.GainAtmosphere : SoundType.LoseAtmosphere, 1f, false);
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
            animLighting.SetBool("isActive", false);
            exitAnim.SetBool("IsOpen", true);
            SoundManager.PlaySound(SoundType.Airlock, 0.5f);

            // Wait for player to leave, then close exit door
            yield return new WaitUntil(() => !playerInside || cancelCycle);
            yield return new WaitForSeconds(1f);
            exitAnim.SetBool("IsOpen", false);
            SoundManager.PlaySound(SoundType.Airlock, 0.5f);

            lastCycleTime = Time.time;
            isCycling = false;
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
            animLighting.SetBool("isActive", false);

            isCycling = false;
            playerInside = false;
            cancelCycle = false;
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