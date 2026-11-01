using UnityEngine;
using System;
using LunarAnomaly.Gameplay;

namespace LunarAnomaly.Player
{
    public class Oxygen : MonoBehaviour
    {   
        [SerializeField] float startingOxygen = 120f;
        [SerializeField] float drainRate = 1f;
        [SerializeField] float refillRate = 2f;
        [SerializeField] bool oxygenRefilling = false;
        public bool oxygenDraining { get; private set; }

        [Header("Debug")]
        public float OxygenDrainRate => drainRate;

        float currentOxygen;
        bool oxygenActive;
        bool oxygenDepleted;
        bool warningsNeedReset = false;

        // Sent to PlayerState
        public static event Action<bool> OnOxygenDepleted; 
        // Sent to OxygenUIManager
        public static event Action<float> OnOxygenChanged;
        public static event Action OnOxygenUIReset;

        void OnEnable()
        {
            AtmosphereTracker.OnPressurized += AtmosphereUpdated;
            OutpostRevealCinematic.OnUpdateOxygen += UpdateCurrentOxygen;
        }

        void OnDisable()
        {
            AtmosphereTracker.OnPressurized -= AtmosphereUpdated;
            OutpostRevealCinematic.OnUpdateOxygen -= UpdateCurrentOxygen;
        }

        void Start()
        {
            oxygenDraining = true;
            oxygenActive = true;
            currentOxygen = startingOxygen;
        }

        void Update()
        {
            UpdateOxygen();        
        }

        void UpdateOxygen()
        {
            if (!oxygenActive) return; 

            if (oxygenDraining)
            {
                DrainOxygen();
            }

            if (oxygenRefilling)
            {
                RefillOxygen();
            }
        }

        void UpdateCurrentOxygen(float newOxygen)
        {
            currentOxygen = startingOxygen * newOxygen;
        }

        void DrainOxygen()
        {
            // Cap to 0 min
            currentOxygen = Mathf.Max(0f, currentOxygen - drainRate * Time.deltaTime);

                                    // Sends fill %
            OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);

            if (currentOxygen <= startingOxygen * 0.5f) warningsNeedReset = true;
            if (currentOxygen <= 0) OxygenDepleted();
        }

        void RefillOxygen()
        {
            if (oxygenDepleted && currentOxygen > 0)
            {
                oxygenDepleted = false;
                OnOxygenDepleted?.Invoke(false);
            }
            if (currentOxygen < startingOxygen)
            {
                currentOxygen = Mathf.Min(startingOxygen, currentOxygen + refillRate * Time.deltaTime);               
                OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);
            }

            if (currentOxygen >= startingOxygen * 0.6f && warningsNeedReset)
            {
                OnOxygenUIReset?.Invoke(); 
                warningsNeedReset = false;
            }
        }

        void OxygenDepleted()
        {
            if (!oxygenDepleted && currentOxygen <= 0)
            {
                oxygenDepleted = true;
                OnOxygenDepleted?.Invoke(true);
            }  
        }

        void AtmosphereUpdated(bool pressurized)
        {
            Debug.LogWarning($"Oxygen received OnPressurized: {pressurized} | " + $"Oxygen: {currentOxygen}");
            
            if (pressurized)
            {
                oxygenDraining = false;
                oxygenRefilling = true;
            }
            else
            {
                oxygenDraining = true;
                oxygenRefilling = false;
            }
        }    

        public void SetActive(bool active)
        {
            oxygenActive = active;
        }

        // Called from PlayerState during Death
        public void ResetOxygen()
        {
            currentOxygen = startingOxygen; 
            OnOxygenUIReset?.Invoke(); 
            OnOxygenChanged?.Invoke(startingOxygen);
            warningsNeedReset = false;
            oxygenDepleted = false;
            oxygenActive = true;
        }

        public void UpdateOxygenDrainRate(float rate)
        {
            drainRate = rate;
        }
    }
}
