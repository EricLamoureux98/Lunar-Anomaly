using System;
using LunarAnomaly.UI;
using UnityEngine;

public class HabitatController : MonoBehaviour
{

    // To HabitatAirlock
    public static event Action OnEnterHabitat;
    public static event Action OnExitHabitat;

    void OnEnable()
    {
        HabitatTriggerZone.OnInteract += HandleInteract;
    }

    void OnDisable()
    {
        HabitatTriggerZone.OnInteract -= HandleInteract;
    }

    void HandleInteract(HabitatPrompt prompt)
    {
        switch (prompt)
        {
            case HabitatPrompt.EnterBase:
                OnEnterHabitat?.Invoke();
                break;

            case HabitatPrompt.ExitBase:
                OnExitHabitat?.Invoke();
                break;
        }
    }
}
