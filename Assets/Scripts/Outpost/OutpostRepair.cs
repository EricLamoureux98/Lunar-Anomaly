using System;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostRepair : MonoBehaviour
	{
		[SerializeField] RepairNode[] repairNodes;

		//int repairedCount;

		//public static event Action OnOutpostRepaired;

        // To ObjectiveManager - Used in PipeValve
        public static Action<ProgressionStage> OnOutpostRepairProgress;

        void OnEnable()
        {
            foreach (var node in repairNodes)
			{
				node.OnNodeRepaired += HandleNodeRepaired;
			}
        }

        void OnDisable()
        {
            foreach (var node in repairNodes)
			{
				node.OnNodeRepaired -= HandleNodeRepaired;
			}
        }

		void HandleNodeRepaired(RepairNode node)
		{
            OnOutpostRepairProgress?.Invoke(ProgressionStage.OutpostObjective);
			// repairedCount++;

			// if (repairedCount >= repairNodes.Length)
			// {
			// 	OnOutpostRepaired?.Invoke();
			// }
		}
    }
}
