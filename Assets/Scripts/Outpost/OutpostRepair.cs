using System;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostRepair : MonoBehaviour
	{
		[SerializeField] RepairNode[] repairNodes;

		//int repairedCount;

		//public static event Action OnOutpostRepaired;

        // To ObjectiveManager - Used in PipeValve & SolarPanel
        public static Action<ProgressionStage> OnOutpostRepairProgress;

		public static event Action OnSolarPanelRepaired;

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
			// There is only the solar panel to repair at this point
            OnSolarPanelRepaired?.Invoke();

			// repairedCount++;

			// if (repairedCount >= repairNodes.Length)
			// {
			// 	OnOutpostRepaired?.Invoke();
			// }
		}
    }
}
