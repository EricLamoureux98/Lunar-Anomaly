using System;
using UnityEngine;
using UnityEngine.UI;


namespace LunarAnomaly.Gameplay
{
	public class RepairNode : MonoBehaviour
	{
		[SerializeField] GameObject brokenPrefab, repairedPrefab;
		[SerializeField] Image repairBar;
		//[SerializeField] Collider repairCollider;

		[SerializeField] float repairTime = 4f;
		float currentRepairAmount;
		public bool canBeRepaired { get; private set; }

		// To OutpostRepair
		// Not static so each node owns its own event
		public event Action<RepairNode> OnNodeRepaired;

        void Start()
        {
            canBeRepaired = true;
        }

        public void RepairCurrentNode()
		{
			if (!canBeRepaired) return;

			if (currentRepairAmount < repairTime)
			{
				currentRepairAmount += Time.deltaTime;
				repairBar.fillAmount = currentRepairAmount / repairTime;
				return;
			}

			canBeRepaired = false;
			brokenPrefab.SetActive(false);
			repairedPrefab.SetActive(true);
			OnNodeRepaired?.Invoke(this);
		}
	}
}
