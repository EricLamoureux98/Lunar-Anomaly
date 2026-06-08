using LunarAnomaly.Gameplay;
using UnityEngine;

	
namespace LunarAnomaly
{
	public class AnimEventRelay : MonoBehaviour
	{
		[Header("Outpost")]
		[SerializeField] OutpostController outpostController;
		[SerializeField] ObjectiveManager objectiveManager;
		[SerializeField] PipeValve pipeValve;

		[Header("Habitat")]
		[SerializeField] HabitatAirlock habitatAirlock;

		void OnPowerPanelOpen() => outpostController.HandlePowerboxOpen();
		void OnPowerSwitchFlipped() => outpostController.HandlePowerSwitchSound();
		//void OnOutpostPowerOn() => outpostController.HandleOutpostStart();
		void OnOutpostPowerOn() => objectiveManager.AdvanceObjective(ProgressionStage.OutpostObjective);
		//void OnDishLeverEnabled() => outpostController.HandleDishEnable();
		void OnDishLeverEnabled() => objectiveManager.AdvanceObjective(ProgressionStage.OutpostObjective);

		void OnValveEnabled() => pipeValve.NotifyValveRepaired();

		// Habitat

		void OnAirlockExtDoorNoCollision() => habitatAirlock.ExternalDoorColliderActive(false);
		void OnAirlockExtDoorCollision() => habitatAirlock.ExternalDoorColliderActive(true);

		void OnAirlockIntDoorNoCollision() => habitatAirlock.InternalDoorColliderActive(false);
		void OnAirlockIntDoorCollision() => habitatAirlock.InternalDoorColliderActive(true);
	}
}
