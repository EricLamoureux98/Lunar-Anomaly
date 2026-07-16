using TMPro;
using UnityEngine;

public class HabitatUI : MonoBehaviour
{
    
    [SerializeField] TMP_Text enterHabitat;
    [SerializeField] TMP_Text exitHabitat;
    [SerializeField] TMP_Text pickupPickaxe;
    [SerializeField] TMP_Text pickupWrench;

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
        }
    }
}

public enum HabitatPrompt
{
    EnterHabitat,
    ExitHabitat,
    PickupWrench,
    PickupPickaxe
}
