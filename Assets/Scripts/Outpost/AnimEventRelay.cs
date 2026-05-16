using LunarAnomaly.Gameplay;
using UnityEngine;

	
namespace LunarAnomaly
{
	public class AnimEventRelay : MonoBehaviour
	{
		[SerializeField] OutpostController outpostController;
		[SerializeField] ObjectiveManager objectiveManager;

		void OnPowerPanelOpen() => outpostController.HandlePowerboxOpen();
		void OnPowerSwitchFlipped() => outpostController.HandlePowerSwitchSound();
		//void OnOutpostPowerOn() => outpostController.HandleOutpostStart();
		void OnOutpostPowerOn() => objectiveManager.AdvanceObjective(ProgressionStage.OutpostObjective);
		//void OnDishLeverEnabled() => outpostController.HandleDishEnable();
		void OnDishLeverEnabled() => objectiveManager.AdvanceObjective(ProgressionStage.OutpostObjective);
	}
}
