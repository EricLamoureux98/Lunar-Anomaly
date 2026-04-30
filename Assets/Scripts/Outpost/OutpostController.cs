using System;
using System.Collections;
using LunarAnomaly.UI;
using Unity.Cinemachine;
using UnityEngine;
	
namespace LunarAnomaly.Gameplay
{
	public class OutpostController : MonoBehaviour
	{	
		[SerializeField] Animator powerBoxDoorAnim;
		[SerializeField] Animator powerSwitchesAnim;
		[SerializeField] CinemachineImpulseSource powerImpulseSource;
		[SerializeField] Renderer doorLight;
		[SerializeField] Renderer buttonLight;
		OutpostAirlock outpostAirlock;

		

		[SerializeField] bool debugIsPowered;
		bool outpostActive;
		bool isPowered;
		public bool outpostRepaired; // For testing
		bool powerBoxOpen;

		// To OutpostUI
		public static Action<OutpostPrompt, bool> OnOutpostUIUpdate;
		public static event Action OnOutpostHideAllUI;

		// To OutpostTriggerZone
		public static Action<OutpostPrompt, bool> OnTriggerZoneActive;

        void Awake()
        {
            outpostAirlock = GetComponentInChildren<OutpostAirlock>();
        }

        void OnEnable()
        {
            OutpostRepair.OnOutpostRepaired += HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract += HandleInteract;
        }

        void OnDisable()
        {
            OutpostRepair.OnOutpostRepaired -= HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract -= HandleInteract;
        }

        void Update()
        {
            if (debugIsPowered && !isPowered)
			{
				HandleInteract(OutpostPrompt.TurnOnPower);
			}
        }

        // ***** FINISH THIS ******
        void HandleInteract(OutpostPrompt prompt)
		{
			switch (prompt)
			{
				case OutpostPrompt.PowerPanel:
					TryOpenPowerbox();
					break;

				case OutpostPrompt.TurnOnPower:
					TryEnablePower();
					break;
				
				case OutpostPrompt.EnterOutpost:
					TryEnterOutpost();
					break;
				
				case  OutpostPrompt.ExitOutpost:
					TryExitOutpost();
					break;
			}
		}

		void HandleOutpostRepaired()
		{
			outpostRepaired = true;
		}

		void TryOpenPowerbox()
		{
			if (powerBoxOpen) return;

			powerSwitchesAnim.SetBool("IsPowered", false);
			//OutpostDoorAnim.SetBool("IsOpen", false);
			outpostAirlock.HandleDoorOpen(false);
			powerBoxOpen = true;
			powerBoxDoorAnim.SetBool("IsOpen", true);
			SoundManager.PlaySound(SoundType.OutpostSqueak, 1f, false);			
			OnOutpostUIUpdate?.Invoke(OutpostPrompt.PowerPanel, false);
		}

		// Called from door animation
		public void HandlePowerboxOpen()
		{
			if (!powerBoxOpen) return;

			SoundManager.PlaySound(SoundType.OutpostBang, 1f, false);
			CameraShakeManager.Instance.CameraShake(powerImpulseSource, 0.03f);
			OnOutpostUIUpdate?.Invoke(OutpostPrompt.TurnOnPower, true);
			OnTriggerZoneActive?.Invoke(OutpostPrompt.TurnOnPower, true);
		}

        void TryEnablePower()
		{
			if (outpostRepaired && !isPowered)
			{
				powerSwitchesAnim.SetBool("IsPowered", true);
				isPowered = true;
				OnOutpostUIUpdate?.Invoke(OutpostPrompt.TurnOnPower, false);
				OnTriggerZoneActive?.Invoke(OutpostPrompt.TurnOnPower, false);

				doorLight.material.SetColor("_BaseColor", Color.green);
				doorLight.material.SetColor("_EmissionColor", Color.green);

				buttonLight.material.SetColor("_BaseColor", Color.red);
				buttonLight.material.SetColor("_EmissionColor", Color.red * 2.5f);

				OnOutpostUIUpdate?.Invoke(OutpostPrompt.EnterOutpost, true);
				OnTriggerZoneActive?.Invoke(OutpostPrompt.EnterOutpost, true);
			}
			// Maybe notify player if not fully repaired
		}

		// Called from switch animation
		public void HandlePowerSwitchSound()
		{
			SoundManager.PlaySound(SoundType.SwitchFlip, 2f, false);
		}

		public void HandleOutpostStartSound()
		{
			SoundManager.PlaySound(SoundType.MachineStart, 3f, false);
		}

		// Interior button
		void TryActivateOutpost()
		{
			if (isPowered && !outpostActive)
			{
				outpostActive = true;
				// Handle insanity start
				// Quest complete
			}
		}

		void TryEnterOutpost()
		{
			if (isPowered)
			{
				//Debug.Log("Door opening");
				outpostAirlock.HandleDoorOpen(true);
				SoundManager.PlaySound(SoundType.Airlock, 0.5f, false);
			}
		}

		void TryExitOutpost()
		{
			if (isPowered)
			{
				outpostAirlock.TryExitOutpost();
			}
		}
    }
}


// Give the player a reason to repair the outpost. "Something caused it to break down" 

// Player repairs Outpost exterior
// Power box can be used
// Player enables power
// Door becomes active and can be opened
// Player enters
// Outpost pressurizes and door closes
// Player can activate Outpost
// Player presses activate button
// Power flickers and sounds of powering up
// "Log downloaded" - voice clip starts to play
// Player goes to exit and open door
// Door opens and a silhouette is seen for a short moment
// Cut to black and play insanity sounds. Silhouette is gone
// Insanity system starts 0 - 100%
// Return to base
