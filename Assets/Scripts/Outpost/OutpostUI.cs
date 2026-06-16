using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using TMPro;
using UnityEngine;

	
namespace LunarAnomaly.UI
{
	public class OutpostUI : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] UpdateText updateText;
		[SerializeField] GameObject logBGPanel;
		[SerializeField] CanvasGroup logGroup;
		[SerializeField] TMP_Text currentTextBox;
		[SerializeField] Typewriter typewriter;
		// [SerializeField] PlayerLook playerLook;

		[Header("Power Panel")]
		[SerializeField] TMP_Text openPowerPanel;
		[SerializeField] TMP_Text turnOnPower;
		
		[SerializeField] TMP_Text openDoorExterior;
		[SerializeField] TMP_Text openDoorInterior;
		[SerializeField] TMP_Text activateOutpost;
		[SerializeField] TMP_Text viewLog;
		[SerializeField] TMP_Text pullLever;	
		[SerializeField] TMP_Text turnValve;
		[SerializeField] TMP_Text pickupCable;
		[SerializeField] TMP_Text connectCable;

		[Header("Logs")]
		[TextArea(5,10)]
		[SerializeField] string logText;	

		// To PlayerLook
		public static Action<bool> OnLogShown; // Temp no event for Demo

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

				case OutpostPrompt.TurnValve:
					turnValve.enabled = isActive;
					break;
				
				case OutpostPrompt.PickupCable:
					pickupCable.enabled = isActive;
					break;

				case OutpostPrompt.ConnectCable:
					connectCable.enabled = isActive;
					break;
			}
		}

		void HideAll()
		{
			activateOutpost.enabled = false;
			openPowerPanel.enabled = false;
			turnOnPower.enabled = false;
			openDoorInterior.enabled = false;
			openDoorExterior.enabled = false;
			viewLog.enabled = false;
			pullLever.enabled = false;
			turnValve.enabled = false;
			pickupCable.enabled = false;
			connectCable.enabled = false;
		}

		public void ShowLog()
		{
			logBGPanel.SetActive(true);
			logGroup.alpha = 1f;
			logGroup.interactable = true;
			logGroup.blocksRaycasts = true;

			//PlayerState.OnHideGameplayUI?.Invoke(false);
			//playerLook.UpdateCursorLock(true);
			OnLogShown?.Invoke(true);
		
			updateText.UpdateCurrentTextBox(currentTextBox);
			updateText.ShowWithTypewriter(logText);
		}

		public void CloseLog()
		{
			logBGPanel.SetActive(false);
			logGroup.alpha = 0f;
			logGroup.interactable = false;
			logGroup.blocksRaycasts = false;

			//PlayerState.OnHideGameplayUI?.Invoke(true);
			// playerLook.UpdateCursorLock(false);
			OnLogShown?.Invoke(false);
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
		DishLever, 
		PickupValve,
		ConnectValve,
		TurnValve,
		PickupCable,
		ConnectCable
	}
}
