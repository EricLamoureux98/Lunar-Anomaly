using System;
using LunarAnomaly.Gameplay;
using UnityEngine;

namespace LunarAnomaly.Player
{
    public class AtmosphereTracker : MonoBehaviour
    {
        bool IsPressurized;

        // Sent to Oxygen
        public static event Action<bool> OnPressurized;

        void OnEnable()
        {
            AtmosphereZone.OnZonePressureChanged += HandlePressureChanged;
            PlayerState.OnResetPressure += RefreshPressureState;
        }

        void OnDisable()
        {
            AtmosphereZone.OnZonePressureChanged -= HandlePressureChanged;
            PlayerState.OnResetPressure -= RefreshPressureState;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out AtmosphereZone zone))
            {
                if (IsPressurized == zone.IsPressurized) return;
                
                IsPressurized = zone.IsPressurized;
                OnPressurized?.Invoke(IsPressurized);
            }
        }

        void RefreshPressureState()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent(out AtmosphereZone zone))
                {
                    if (IsPressurized == zone.IsPressurized) return;

                    IsPressurized = zone.IsPressurized;
                    OnPressurized?.Invoke(IsPressurized);
                    
                    Debug.Log("Refresh Success. Player inside pressurized zone");
                }
            }
        }

        void HandlePressureChanged(bool pressurized)
        {
            OnPressurized?.Invoke(pressurized);
        }
    }
}