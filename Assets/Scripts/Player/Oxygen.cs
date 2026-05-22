using UnityEngine;
using System;

namespace LunarAnomaly.Player
{
    public class Oxygen : MonoBehaviour
    {   
        [SerializeField] float startingOxygen = 120f;
        [SerializeField] float drainRate = 1f;
        [SerializeField] float refillRate = 2f;
        [SerializeField] bool oxygenRefilling = false;
        public bool oxygenDraining { get; private set; }

        float currentOxygen;
        bool oxygenActive;
        bool oxygenDepleted;

        // Sent to PlayerState
        public static event Action<bool> OnOxygenDepleted; 

        // Sent to UIManager
        public static event Action<float> OnOxygenChanged;
        public static event Action OnOxygenReset;

        void OnEnable()
        {
            AtmosphereTracker.OnPressurized += AtmosphereUpdated;
        }

        void OnDisable()
        {
            AtmosphereTracker.OnPressurized -= AtmosphereUpdated;
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

        void DrainOxygen()
        {
            // Cap to 0 min
            currentOxygen = Mathf.Max(0f, currentOxygen - drainRate * Time.deltaTime);

                                    // Sends fill %
            OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);

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
                currentOxygen += refillRate * Time.deltaTime;          
                OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);
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
            if (pressurized)
            {
                //Debug.Log("Player entered pressurized area");
                oxygenDraining = false;
                oxygenRefilling = true;
            }
            else
            {
                //Debug.Log("Player exited pressurized area");
                oxygenDraining = true;
                oxygenRefilling = false;
            }
        }    

        public void SetActive(bool active)
        {
            oxygenActive = active;
        }

        public void ResetOxygen()
        {
            currentOxygen = startingOxygen; 
            OnOxygenReset?.Invoke(); 
            oxygenDepleted = false;
            oxygenActive = true;
        }
    }
}
