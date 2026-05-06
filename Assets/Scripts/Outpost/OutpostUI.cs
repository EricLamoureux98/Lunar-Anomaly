using LunarAnomaly.Gameplay;
using TMPro;
using UnityEngine;

	
namespace LunarAnomaly.UI
{
	public class OutpostUI : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] TerminalUpdateText updateText;
		
		[Header("Power Panel")]
		[SerializeField] TMP_Text openPowerPanel;
		[SerializeField] TMP_Text turnOnPower;
		
		//[SerializeField] TMP_Text repairNode;
		[SerializeField] TMP_Text openDoorExterior;
		[SerializeField] TMP_Text openDoorInterior;
		[SerializeField] TMP_Text activateOutpost;
		[SerializeField] TMP_Text viewLog;
		[SerializeField] TMP_Text pullLever;

        // Typewriter stuff for audio log portion

        void OnEnable()
        {
            OutpostController.OnOutpostUIUpdate += HandleUIUpdate;
			OutpostController.OnOutpostHideAllUI += HideAll;
        }

        void OnDisable()
        {
            OutpostController.OnOutpostUIUpdate -= HandleUIUpdate;
			OutpostController.OnOutpostHideAllUI -= HideAll;
        }

        void HandleUIUpdate(OutpostPrompt prompt, bool isActive)
		{
			switch (prompt)
			{
				case OutpostPrompt.PowerPanel:
					openPowerPanel.enabled = isActive;
					break;

				case OutpostPrompt.TurnOnPower:
					turnOnPower.enabled = isActive;
					break;

				case OutpostPrompt.EnterOutpost:
					openDoorExterior.enabled = isActive;
					break;

				case OutpostPrompt.ExitOutpost:
					openDoorInterior.enabled = isActive;
					break;

				case OutpostPrompt.ActivateOutpost:
					activateOutpost.enabled = isActive;
					break;

				case OutpostPrompt.ViewLog:
					viewLog.enabled = isActive;
					break;

				case OutpostPrompt.DishLever:
				pullLever.enabled = isActive;
				break;
			}
		}

		void HideAll()
		{
			activateOutpost.enabled = false;
			openPowerPanel.enabled = false;
			turnOnPower.enabled = false;
			//repairNode.enabled = false;
			openDoorInterior.enabled = false;
			openDoorExterior.enabled = false;
			viewLog.enabled = false;
			pullLever.enabled = false;
		}
	}

	public enum OutpostPrompt
	{
		PowerPanel,
		TurnOnPower,
		EnterOutpost,
		ActivateOutpost,
		ViewLog,
		Basic,
		ExitOutpost,
		UseLadder,
		DishLever
	}
}
