using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.UI;
using UnityEngine;

public class HabitatController : MonoBehaviour
{
    [Header("Tools")]
    [SerializeField] GameObject pickaxeObj;
    [SerializeField] GameObject wrenchObj;

    [SerializeField] Transform helmetTransform;
    //DiscoveryZone discoveryZone;

    // To HabitatAirlock
    public static event Action OnEnterHabitat;
    public static event Action OnExitHabitat;
    // To ObjectiveManager
    public static event Action<ProgressionStage> OnHabitatProgress;
    // To HabitatTriggerZone
	public static Action<HabitatPrompt, bool> OnTriggerZoneActive;
    // To HabitatUI
	public static Action<HabitatPrompt, bool> OnHabitatUIUpdate;
    // To WaypointManager - Used in OutpostController
    public static Action<Transform> OnUpdateWaypointTarget;
    public static Action<bool> OnUpdateWaypointActive;

    bool firstTimeExit = true;

    void Awake()
    {
        //discoveryZone = GetComponentInChildren<DiscoveryZone>();
    }

    void OnEnable()
    {
        HabitatTriggerZone.OnInteract += HandleInteract;
        DiscoveryZone.OnHabitatZoneEntered += PrepareMiningObjective;
    }

    void OnDisable()
    {
        HabitatTriggerZone.OnInteract -= HandleInteract;
        DiscoveryZone.OnHabitatZoneEntered -= PrepareMiningObjective;
    }

    void HandleInteract(HabitatPrompt prompt)
    {
        switch (prompt)
        {
            case HabitatPrompt.EnterHabitat:
                OnEnterHabitat?.Invoke();
                break;

            case HabitatPrompt.ExitHabitat:
                OnExitHabitat?.Invoke();
                if (firstTimeExit)
                {
                    OnHabitatProgress?.Invoke(ProgressionStage.OutpostObjective);
                    firstTimeExit = false;
                }
                break;
            
            case HabitatPrompt.PickupWrench:
                ObjectiveManager.OnToolActive?.Invoke(ToolType.repairTool, true);
                OnHabitatProgress?.Invoke(ProgressionStage.OutpostObjective);
                OnTriggerZoneActive?.Invoke(HabitatPrompt.ExitHabitat, true);
                OnHabitatUIUpdate?.Invoke(HabitatPrompt.ExitHabitat, true);
                OnUpdateWaypointTarget?.Invoke(helmetTransform);
                OnUpdateWaypointActive?.Invoke(true);
                wrenchObj.SetActive(false);
                break;

            case HabitatPrompt.PickupPickaxe:
                ObjectiveManager.OnToolActive?.Invoke(ToolType.pickaxe, true);
                //TerminalUI.OnRequestNotification?.Invoke()
                OnHabitatProgress?.Invoke(ProgressionStage.SampleObjective);
                pickaxeObj.SetActive(false);
                break;
        }
    }

    void PrepareMiningObjective()
    {
        Debug.Log("Preparing Mining Objective");
        TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.CollectPickaxe);
        OnTriggerZoneActive?.Invoke(HabitatPrompt.PickupPickaxe, true);
        OnHabitatUIUpdate?.Invoke(HabitatPrompt.PickupPickaxe, true);
    }
}
