using System;
using TMPro;
using UnityEngine;

public class HabitatUI : MonoBehaviour
{
    
    [SerializeField] TMP_Text enterHabitat;
    [SerializeField] TMP_Text exitHabitat;
    [SerializeField] TMP_Text pickupPickaxe;
    [SerializeField] TMP_Text pickupWrench;
    [SerializeField] TMP_Text depositSamples;

    void OnEnable()
    {
        HabitatController.OnHabitatUIUpdate += HandleUIUpdate;
    }

    void OnDisable()
    {
        HabitatController.OnHabitatUIUpdate -= HandleUIUpdate;
    }

    void HandleUIUpdate(HabitatPrompt prompt, bool active)
    {
        switch (prompt)
        {
            case HabitatPrompt.EnterHabitat:
                enterHabitat.enabled = active;
                break;
            
            case HabitatPrompt.ExitHabitat:
                exitHabitat.enabled = active;
                break;
            
            case HabitatPrompt.PickupWrench:
                pickupWrench.enabled = active;
                break;
            
            case HabitatPrompt.PickupPickaxe:
                pickupPickaxe.enabled = active;
                break;
            
            case HabitatPrompt.DepositSamples:
                depositSamples.enabled = active;
                break;
        }
    }
}

public enum HabitatPrompt
{
    EnterHabitat,
    ExitHabitat,
    PickupWrench,
    PickupPickaxe,
    DepositSamples
}
