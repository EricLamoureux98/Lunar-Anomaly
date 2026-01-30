using System;
using UnityEngine;

public class AtmosphereTracker : MonoBehaviour
{
    public bool IsPressurized { get; private set; }

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
