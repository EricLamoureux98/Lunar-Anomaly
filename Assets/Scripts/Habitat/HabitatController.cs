using System;
using LunarAnomaly;
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
    // To MiningManager
    public static event Action OnDepositSamples;

    bool firstTimeExit = true;	

    void Awake()
    {
        //discoveryZone = GetComponentInChildren<DiscoveryZone>();
    }

    void OnEnable()
    {
        HabitatTriggerZone.OnInteract += HandleInteract;
        DiscoveryZone.OnHabitatZoneEntered += PrepareMiningObjective;
        TerminalInterfacePanel.OnIntroProceed += PrepareWrenchObjective;
    }

    void OnDisable()
    {
        HabitatTriggerZone.OnInteract -= HandleInteract;
        DiscoveryZone.OnHabitatZoneEntered -= PrepareMiningObjective;
        TerminalInterfacePanel.OnIntroProceed -= PrepareWrenchObjective;
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
                SoundManager.PlaySound(SoundType.Pickup, 1, false);
                ObjectiveManager.OnToolActive?.Invoke(ToolType.repairTool, true);
                OnHabitatProgress?.Invoke(ProgressionStage.OutpostObjective);
                OnTriggerZoneActive?.Invoke(HabitatPrompt.ExitHabitat, true);
                OnHabitatUIUpdate?.Invoke(HabitatPrompt.ExitHabitat, true);
                OnUpdateWaypointTarget?.Invoke(helmetTransform);
                OnUpdateWaypointActive?.Invoke(true);
                wrenchObj.SetActive(false);
                break;

            case HabitatPrompt.PickupPickaxe:
                SoundManager.PlaySound(SoundType.Pickup, 1, false);
                ObjectiveManager.OnToolActive?.Invoke(ToolType.pickaxe, true);
                //TerminalUI.OnRequestNotification?.Invoke()
                OnHabitatProgress?.Invoke(ProgressionStage.SampleObjective);
                PrepareSampleObjective();
                pickaxeObj.SetActive(false);
                break;

            case HabitatPrompt.DepositSamples:
                DepositSamplesCollected();
                break;
        }
    }

    void PrepareWrenchObjective()
    {
        OnTriggerZoneActive(HabitatPrompt.PickupWrench, true);
        OnHabitatUIUpdate(HabitatPrompt.PickupWrench, true);
    }

    void PrepareMiningObjective()
    {
        // Debug.Log("Preparing Mining Objective");
        TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.CollectPickaxe);
        OnHabitatProgress?.Invoke(ProgressionStage.SampleObjective);
        OnTriggerZoneActive?.Invoke(HabitatPrompt.PickupPickaxe, true);
        OnHabitatUIUpdate?.Invoke(HabitatPrompt.PickupPickaxe, true);
    }

    void PrepareSampleObjective()
    {
        OnTriggerZoneActive(HabitatPrompt.DepositSamples, true);
        OnHabitatUIUpdate(HabitatPrompt.DepositSamples, true);
    }

    void DepositSamplesCollected()
    {
        OnDepositSamples?.Invoke();
    }
}

public enum ToolType
{
    pickaxe,
    repairTool
}