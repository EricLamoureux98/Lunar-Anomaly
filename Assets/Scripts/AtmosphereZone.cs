using System;
using UnityEngine;

public class AtmosphereZone : MonoBehaviour
{
    [SerializeField] bool isPressurized;

    public bool IsPressurized => isPressurized;

    public static event Action<bool> OnZonePressureChanged;

    public void SetPressuized(bool value)
    {
        if (isPressurized == value) return;

        isPressurized = value;
        OnZonePressureChanged?.Invoke(isPressurized);
    }
}
