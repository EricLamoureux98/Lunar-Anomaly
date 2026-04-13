using UnityEngine;

	
namespace LunarAnomaly
{
	public class Outpost : MonoBehaviour
	{
		[SerializeField] GameObject brokenObject, damagedObject, repairedObject;
		[SerializeField] float repairTime = 5f;

		OutpostDamageStage structureStage;
		
		bool canBeRepaired;
		float currentRepairAmount;

        // Add event for when structure is fully repaired
        // Player will need to enter and activate
        // Sanity takes a turn at this point

        void Start()
        {
            canBeRepaired = true;
			AdvanceStage(OutpostDamageStage.broken);
        }

		public void RepairStructure()
		{
			if (currentRepairAmount < repairTime && canBeRepaired)
			{
				currentRepairAmount += Time.deltaTime;
			}
			else
			{
				UpdateRepairStage();
				currentRepairAmount = 0f;
			}
		}

        // Call from Repair Script event later
        void UpdateRepairStage()
		{
			if (!canBeRepaired) return;
			AdvanceStage(GetNextStage(structureStage));
		}

		OutpostDamageStage GetNextStage(OutpostDamageStage stage)
		{
			switch (stage)
			{
				case OutpostDamageStage.broken:
					return OutpostDamageStage.damaged;

				case OutpostDamageStage.damaged:
					return OutpostDamageStage.repaired;

				case OutpostDamageStage.repaired:
					return stage;

				default:
					return stage;
			}
		}

		void AdvanceStage(OutpostDamageStage newStage)
		{
			if (newStage == structureStage) return;

			ExitStage(structureStage);
			structureStage = newStage;
			EnterStage(newStage);
		}

		void EnterStage(OutpostDamageStage stage)
		{
			switch (stage)
			{
				case OutpostDamageStage.broken:
					brokenObject.SetActive(true);
					damagedObject.SetActive(false);
					repairedObject.SetActive(false);
					break;

				case OutpostDamageStage.damaged:
					damagedObject.SetActive(true);
					break;

				case OutpostDamageStage.repaired:
					repairedObject.SetActive(true);
					canBeRepaired = false;
					break;
			}
		}

		void ExitStage(OutpostDamageStage stage)
		{
			switch (stage)
			{
				case OutpostDamageStage.broken:
					damagedObject.SetActive(false);
					repairedObject.SetActive(false);
					break;

				case OutpostDamageStage.damaged:
					brokenObject.SetActive(false);
					repairedObject.SetActive(false);
					break;

				case OutpostDamageStage.repaired:
					break;
			}
		}
	}

	public enum OutpostDamageStage
	{
		broken,
		damaged,
		repaired
	}
}
