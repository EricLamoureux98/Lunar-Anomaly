using LunarAnomaly.Gameplay;
using UnityEngine;

	
namespace LunarAnomaly
{
	public class AnimEventRelay : MonoBehaviour
	{
		[SerializeField] OutpostController outpostController;

		void OnPowerPanelOpen() => outpostController.HandlePowerboxOpen();
		void OnPowerSwitchFlipped() => outpostController.HandlePowerSwitchSound();
		void OnOutpostPowerOn() => outpostController.HandleOutpostStartSound();
	}
}
