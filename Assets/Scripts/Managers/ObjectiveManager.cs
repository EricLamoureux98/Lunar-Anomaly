using System;
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

        void OnEnable()
        {
            ProgressionManager.OnStageChanged += UpdateObjective;
            OutpostRepair.OnOutpostProgress += UpdateProgress;
            OutpostController.OnOutpostAdvanced += AdvanceObjective;
            HabitatController.OnHabitatProgress += AdvanceObjective;
            //MiningManager.OnSamplesCarriedChanged += UpdateProgress;
        }

        void OnDisable()
        {
            ProgressionManager.OnStageChanged -= UpdateObjective;
            OutpostRepair.OnOutpostProgress -= UpdateProgress;
            OutpostController.OnOutpostAdvanced -= AdvanceObjective;
            HabitatController.OnHabitatProgress -= AdvanceObjective;
            //MiningManager.OnSamplesCarriedChanged -= UpdateProgress;
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
                    currentStage = newStage;
                    currentObjectiveIndex = 0;
                    currentObjectiveSO = noObjectiveSO;
                    OnUpdateObjectiveTitle?.Invoke(noObjectiveSO.Objectives[0].Title);
                    PrepareObjectiveData(noObjectiveSO, noObjectiveSO.Objectives[0].objectiveType);
                    OnUpdateObjectiveData?.Invoke(objectiveUIData);  
                    OnToolActive?.Invoke(ToolType.pickaxe, false);
                    OnToolActive?.Invoke(ToolType.repairTool, false);
                    break;

                case ProgressionStage.OutpostObjective:
                    if (outpostSO == null ) return;
                    currentStage = newStage;
                    currentObjectiveIndex = 0;
                    currentProgress = 0;
                    currentObjectiveSO = outpostSO;
                    OnUpdateObjectiveTitle?.Invoke(outpostSO.Objectives[0].Title);
                    PrepareObjectiveData(outpostSO, outpostSO.Objectives[0].objectiveType);
                    OnUpdateObjectiveData?.Invoke(objectiveUIData);
                    break;

                case ProgressionStage.SampleObjective:
                    currentObjectiveSO = miningSO;
                    break;                
            }
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
            OnUpdateObjectiveTitle?.Invoke(outpostSO.Objectives[currentObjectiveIndex].Title);
            PrepareObjectiveData(currentObjectiveSO, currentObjectiveSO.Objectives[currentObjectiveIndex].objectiveType);
            OnUpdateObjectiveData?.Invoke(objectiveUIData);
        }    

        void PrepareObjectiveData(ObjectiveSO currentSO, ObjectiveType currentType)
        {
            ObjectiveUIData data = new ObjectiveUIData
            {
                Type = currentType,
                Progress = currentProgress,
                Remaining = currentSO.Objectives[currentObjectiveIndex].Progression.AmountNeeded,
                TypeText = currentSO.Objectives[currentObjectiveIndex].Progression.ProgressionName
            };

            objectiveUIData = data;
        }
    }
}

public struct ObjectiveUIData
{
    public ObjectiveType Type;
    public int? Progress;
    public int? Remaining;
    public string TypeText;
}
