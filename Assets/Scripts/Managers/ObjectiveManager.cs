using System;
using LunarAnomaly.Player;
using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class ObjectiveManager : MonoBehaviour
    {
        // Replace these later with a Dictionary
        [SerializeField] ObjectiveSO outpostSO;
        [SerializeField] ObjectiveSO miningSO;
        [SerializeField] ObjectiveSO noObjectiveSO;
        [SerializeField] ObjectiveSO anomalySO;

        int currentObjectiveIndex;
        int currentProgress;

        ObjectiveUIData objectiveUIData;
        ObjectiveSO currentObjectiveSO;
        ProgressionStage currentStage;

        // To ObjectiveUIController
        public static event Action<string> OnUpdateObjectiveTitle;
        public static event Action<ObjectiveUIData> OnUpdateObjectiveData;
        // To OutpostController
        public static event Action<ProgressionStage, int> OnObjectiveProgressed;
        // To Pickaxe and RepairTool - Used in HabitatToolPickup
		public static Action<ToolType, bool> OnToolActive;
        // To MiningManager
        public static event Action<int> OnBeginMiningObjective;
        // To PlayerState
        public event Action<RespawnPoint> OnUpdateRespawnPoint;

        void OnEnable()
        {
            ProgressionManager.OnStageChanged += UpdateObjective;
            OutpostRepair.OnOutpostProgress += UpdateProgress;
            OutpostController.OnOutpostAdvanced += AdvanceObjective;
            HabitatController.OnHabitatProgress += AdvanceObjective;
            // MiningManager.OnSamplesCarriedChanged += UpdateProgress;
            MiningManager.OnDepositProgressChanged += HandleMiningProgress;
        }

        void OnDisable()
        {
            ProgressionManager.OnStageChanged -= UpdateObjective;
            OutpostRepair.OnOutpostProgress -= UpdateProgress;
            OutpostController.OnOutpostAdvanced -= AdvanceObjective;
            HabitatController.OnHabitatProgress -= AdvanceObjective;
            // MiningManager.OnSamplesCarriedChanged -= UpdateProgress;
            MiningManager.OnDepositProgressChanged -= HandleMiningProgress;
        }

        void Start()
        {
            UpdateObjective(ProgressionStage.Intro);
        }

        void UpdateObjective(ProgressionStage newStage)
        {
            switch (newStage)
            {
                case ProgressionStage.Intro:
                if (noObjectiveSO == null ) return;
                    PrepareNewStage(ProgressionStage.NoObjective, noObjectiveSO);
                    OnToolActive?.Invoke(ToolType.pickaxe, false);
                    OnToolActive?.Invoke(ToolType.repairTool, false);
                    break;

                case ProgressionStage.OutpostObjective:
                    if (outpostSO == null ) return;
                    PrepareNewStage(ProgressionStage.OutpostObjective, outpostSO);
                    OnUpdateRespawnPoint?.Invoke(RespawnPoint.Outpost);
                    break;

                case ProgressionStage.SampleObjective:
                    if (miningSO == null ) return;
                    PrepareNewStage(ProgressionStage.SampleObjective, miningSO);
                    int required = miningSO.Objectives[2].Progression.AmountNeeded; // Make smarter
                    OnBeginMiningObjective?.Invoke(required);
                    OnUpdateRespawnPoint?.Invoke(RespawnPoint.Habitat);
                    break;    

                case ProgressionStage.Anomaly:
                    Debug.Log("Entering Anomaly stage");
                    if (anomalySO == null) return;
                    PrepareNewStage(ProgressionStage.Anomaly, anomalySO);
                    break;
            }
        }

        void PrepareNewStage(ProgressionStage stage, ObjectiveSO objectiveSO)
        {
            currentStage = stage;
            currentObjectiveIndex = 0;
            currentProgress = 0;
            currentObjectiveSO = objectiveSO;
            OnUpdateObjectiveTitle?.Invoke(objectiveSO.Objectives[0].Title);
            PrepareObjectiveData(objectiveSO, objectiveSO.Objectives[0].objectiveType);
            OnUpdateObjectiveData?.Invoke(objectiveUIData);
        }

        void UpdateProgress(ProgressionStage stage)
        {
            if (stage != currentStage) return; 

            currentProgress++;

            PrepareObjectiveData(currentObjectiveSO, currentObjectiveSO.Objectives[currentObjectiveIndex].objectiveType);
            OnUpdateObjectiveData?.Invoke(objectiveUIData);

            if (currentProgress >= currentObjectiveSO.Objectives[currentObjectiveIndex].Progression.AmountNeeded)
            {
                AdvanceObjective(currentStage);
            }
        }

        void HandleMiningProgress(int deposited, int required)
        {
            currentProgress = deposited;

            PrepareObjectiveData(currentObjectiveSO, currentObjectiveSO.Objectives[currentObjectiveIndex].objectiveType);
            OnUpdateObjectiveData?.Invoke(objectiveUIData);

            if (deposited >= required)
            {
                AdvanceObjective(currentStage);
            }
        }

        public void AdvanceObjective(ProgressionStage stage)
        {
            if (stage != currentStage) return; 
            
            currentObjectiveIndex++;

            if (currentObjectiveIndex >= currentObjectiveSO.Objectives.Length)
            {
                Debug.Log("Quest finished!");
                return;
            }
            
            currentProgress = 0;

            OnObjectiveProgressed?.Invoke(currentStage, currentObjectiveIndex);
            OnUpdateObjectiveTitle?.Invoke(currentObjectiveSO.Objectives[currentObjectiveIndex].Title);
            PrepareObjectiveData(currentObjectiveSO, currentObjectiveSO.Objectives[currentObjectiveIndex].objectiveType);
            OnUpdateObjectiveData?.Invoke(objectiveUIData);
        }    

        void PrepareObjectiveData(ObjectiveSO currentSO, ObjectiveType currentType)
        {
            ObjectiveUIData data = new ObjectiveUIData
            {
                Type = currentType,
                Current = currentProgress,
                Required = currentSO.Objectives[currentObjectiveIndex].Progression.AmountNeeded,
                TypeText = currentSO.Objectives[currentObjectiveIndex].Progression.ProgressionName
            };

            objectiveUIData = data;
        }
    }
}

public struct ObjectiveUIData
{
    public ObjectiveType Type;
    public int? Current;
    public int? Required;
    public string TypeText;
}
