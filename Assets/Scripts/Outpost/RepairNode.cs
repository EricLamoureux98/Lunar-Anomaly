using System;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class RepairNode : MonoBehaviour
	{
		[SerializeField] GameObject brokenPrefab, repairedPrefab;

		[SerializeField] float repairTime = 4f;
		float currentRepairAmount;
		bool canBeRepaired = true;

		// To OutpostRepair
		// Not static so each node owns its own event
		public event Action<RepairNode> OnNodeRepaired;

		public void RepairCurrentNode()
		{
			if (!canBeRepaired) return;

			if (currentRepairAmount < repairTime)
			{
				currentRepairAmount += Time.deltaTime;
				return;
			}

			canBeRepaired = false;
			brokenPrefab.SetActive(false);
			repairedPrefab.SetActive(true);
			OnNodeRepaired?.Invoke(this);
		}
	}
}
