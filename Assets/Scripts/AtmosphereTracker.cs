using System;
using UnityEngine;

public class AtmosphereTracker : MonoBehaviour
{
    bool IsPressurized;

    // Sent to Oxygen
    public static event Action<bool> OnPressurized;

    void OnEnable()
    {
        AtmosphereZone.OnZonePressureChanged += HandlePressureChanged;
    }

    void OnDisable()
    {
        AtmosphereZone.OnZonePressureChanged -= HandlePressureChanged;
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

    void HandlePressureChanged(bool pressurized)
    {
        OnPressurized?.Invoke(pressurized);
    }
}
