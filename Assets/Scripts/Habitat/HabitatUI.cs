using TMPro;
using UnityEngine;

public class HabitatUI : MonoBehaviour
{
    
    [SerializeField] TMP_Text enterHabitat;
    [SerializeField] TMP_Text exitHabitat;

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
