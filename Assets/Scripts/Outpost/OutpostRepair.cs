using System;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostRepair : MonoBehaviour
	{
		[SerializeField] RepairNode[] repairNodes;

		int repairedCount;

        //public bool OutpostRepaired { get; private set; }
		public static event Action OnOutpostRepaired;

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
			repairedCount++;
			Debug.Log($"Nodes repaired: {repairedCount}/{repairNodes.Length}");

			if (repairedCount >= repairNodes.Length)
			{
				OnOutpostRepaired?.Invoke();
			}
		}


        // void Start()
        // {
        //     //canBeRepaired = true;
        // 	AdvanceStage(OutpostDamageStage.broken);
        // }


        // void UpdateRepairStage()
        // {
        // 	//if (!canBeRepaired) return;
        // 	AdvanceStage(GetNextStage(outpostStage));
        // }

        // OutpostDamageStage GetNextStage(OutpostDamageStage stage)
        // {
        // 	switch (stage)
        // 	{
        // 		case OutpostDamageStage.broken:
        // 			return OutpostDamageStage.damaged;

        // 		case OutpostDamageStage.damaged:
        // 			return OutpostDamageStage.repaired;

        // 		case OutpostDamageStage.repaired:
        // 			return stage;

        // 		default:
        // 			return stage;
        // 	}
        // }

        // void AdvanceStage(OutpostDamageStage newStage)
        // {
        // 	if (newStage == outpostStage) return;

        // 	ExitStage(outpostStage);
        // 	outpostStage = newStage;
        // 	EnterStage(newStage);
        // }

        // void EnterStage(OutpostDamageStage stage)
        // {
        // 	switch (stage)
        // 	{
        // 		case OutpostDamageStage.broken:
        // 			brokenObject.SetActive(true);
        // 			damagedObject.SetActive(false);
        // 			repairedObject.SetActive(false);
        // 			break;

        // 		case OutpostDamageStage.damaged:
        // 			damagedObject.SetActive(true);
        // 			break;

        // 		case OutpostDamageStage.repaired:
        // 			repairedObject.SetActive(true);
        // 			//canBeRepaired = false;
        // 			OutpostRepaired = true;
        // 			break;
        // 	}
        // }

        // void ExitStage(OutpostDamageStage stage)
        // {
        // 	switch (stage)
        // 	{
        // 		case OutpostDamageStage.broken:
        // 			damagedObject.SetActive(false);
        // 			repairedObject.SetActive(false);
        // 			break;

        // 		case OutpostDamageStage.damaged:
        // 			brokenObject.SetActive(false);
        // 			repairedObject.SetActive(false);
        // 			break;

        // 		case OutpostDamageStage.repaired:
        // 			break;
        // 	}
        // }
    }

	// Consider updating this to nodes that need repairing
	// public enum OutpostDamageStage
	// {
	// 	broken,
	// 	damaged,
	// 	repaired
	// }
}
