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
                // Debug.Log($"Entering Atmoshere Zone. Atmosphere: {zone.IsPressurized}");
                if (IsPressurized == zone.IsPressurized) return;
                
                IsPressurized = zone.IsPressurized;
                OnPressurized?.Invoke(IsPressurized);
            }
        }

        void RefreshPressureState()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, 1f, ~0, QueryTriggerInteraction.Collide);

            // Debug.Log($"Hit count: {hits.Length}");
            // foreach (var hit in hits) 
            // { 
            //     Debug.Log($"Hit: {hit.name} | Parent: {hit.transform.parent?.name}"); 
            // }

            foreach (var hit in hits)
            {
                AtmosphereZone zone = hit.GetComponentInParent<AtmosphereZone>();

                if (zone != null)
                {
                    if (IsPressurized != zone.IsPressurized)
                    {
                        IsPressurized = zone.IsPressurized;
                        OnPressurized?.Invoke(IsPressurized);
                    }

                    return;                                   
                }
            }

            if (IsPressurized)
            {
                IsPressurized = false;
                OnPressurized?.Invoke(false);
            }
        }

        void HandlePressureChanged(bool pressurized)
        {
            Debug.Log($"HandlePressureChanged | " + $"Zone says: {pressurized} | " + $"Tracker before: {IsPressurized}");
            IsPressurized = pressurized;
            OnPressurized?.Invoke(IsPressurized);
        }
    }
}