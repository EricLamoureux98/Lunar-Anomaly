using System;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    [SerializeField] bool isPressurized;

    public static event Action<bool> OnPressurized;

    void Update()
    {
        if (isPressurized)
        {
            OnPressurized?.Invoke(true);
        }
        else
        {
            OnPressurized?.Invoke(false);
        }
    }
}
