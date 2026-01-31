using System;
using UnityEngine;

public class AtmosphereTracker : MonoBehaviour
{
    bool IsPressurized;

    // Sent to Oxygen
    public static event Action<bool> OnPressurized;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out AtmosphereZone zone))
        {
            if (IsPressurized == zone.IsPressurized) return;
            
            IsPressurized = zone.IsPressurized;
            OnPressurized?.Invoke(IsPressurized);
        }
    }
}
