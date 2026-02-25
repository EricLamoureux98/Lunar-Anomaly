using UnityEngine.UI;
using UnityEngine;
using System;

namespace LunarAnomaly.Player
{
    public class Oxygen : MonoBehaviour
    {   
        [SerializeField] float startingOxygen = 120f;
        [SerializeField] float drainRate = 1f;
        [SerializeField] float refillRate = 2f;
        [SerializeField] bool oxygenDraining = false;
        [SerializeField] bool oxygenRefilling = false;

        float currentOxygen;
        bool oxygenActive;

        // Sent to PlayerState
        public static event Action OnOxygenDepleted; 

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
            OxygenLowWarning();

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
            currentOxygen -= drainRate * Time.deltaTime;
                                    // Sends fill %
            OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);

            if (currentOxygen <= 0) OxygenDepleted();
        }

        void RefillOxygen()
        {
            if (currentOxygen < startingOxygen)
            {
                currentOxygen += refillRate * Time.deltaTime;          
                OnOxygenChanged?.Invoke(currentOxygen / startingOxygen);
            }
        }

        void OxygenDepleted()
        {
            OnOxygenDepleted?.Invoke();
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

        void OxygenLowWarning()
        {
            if (currentOxygen <= startingOxygen / 2f)
            {
                //UIManager.Instance.CheckOxygenWarnings(currentOxygen, startingOxygen);
            }
            
            if (currentOxygen <= (startingOxygen * 0.1f))
            {
                //UIManager.Instance.CheckOxygenWarnings(currentOxygen, startingOxygen);
            }
        }    

        public void SetActive(bool active)
        {
            oxygenActive = active;
        }

        public void ResetOxygen()
        {
            currentOxygen = startingOxygen; 
            //UIManager.Instance.UpdateOxygenBar(startingOxygen);
            OnOxygenReset?.Invoke(); 
            //UIManager.Instance.ResetOxygenWarnings();
            oxygenActive = true;
        }
    }
}
