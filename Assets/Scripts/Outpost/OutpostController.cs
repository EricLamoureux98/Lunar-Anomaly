using System;
using System.Collections;
using LunarAnomaly.UI;
using Unity.Cinemachine;
using UnityEngine;
	
namespace LunarAnomaly.Gameplay
{
	public class OutpostController : MonoBehaviour
	{	
		[Header("Animators")]
		[SerializeField] Animator powerBoxDoorAnim;
		[SerializeField] Animator powerSwitchesAnim;
		[SerializeField] Animator satelliteDishAnim;
		[SerializeField] Animator dishLeverAnim;
		[SerializeField] Animator bigButtonAnim;

		[SerializeField] CinemachineImpulseSource powerImpulseSource;
		[SerializeField] Renderer doorLight;
		[SerializeField] Renderer buttonLight;
		[SerializeField] Transform ladderTopPosition;
		[SerializeField] GameObject logCubeObj;
		OutpostAirlock outpostAirlock;		
		OutpostUI outpostUI;

		[SerializeField] bool debugIsPowered;
		bool outpostActive;
		bool isPowered;
		public bool dishEnabled;
		public bool outpostRepaired; // For testing
		bool powerBoxOpen;

		// To OutpostUI
		public static Action<OutpostPrompt, bool> OnOutpostUIUpdate;
		public static event Action OnOutpostHideAllUI;

		// To OutpostTriggerZone
		public static Action<OutpostPrompt, bool> OnTriggerZoneActive;

		// To PlayerState
		public static event Action<Transform> OnLadderUsed;

		// To OutpostRevealCinematic
		public static event Action OnCinematicSilhouetteSpawn;

        void Awake()
        {
            outpostAirlock = GetComponentInChildren<OutpostAirlock>();
			outpostUI = GetComponent<OutpostUI>();
        }

        void OnEnable()
        {
            //OutpostRepair.OnOutpostRepaired += HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract += HandleInteract;
			OutpostRevealCinematic.OnDisableOutpost += DisableOutpost;
        }

        void OnDisable()
        {
            //OutpostRepair.OnOutpostRepaired -= HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract -= HandleInteract;
			OutpostRevealCinematic.OnDisableOutpost -= DisableOutpost;
        }

        void Update()
        {
            if (debugIsPowered && !isPowered)
			{
				HandleInteract(OutpostPrompt.TurnOnPower);
			}
        }

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
				
				case OutpostPrompt.UseLadder:
					OnLadderUsed?.Invoke(ladderTopPosition);
					break;

				case OutpostPrompt.DishLever:
					TryDishLever();
					break;

				case OutpostPrompt.ActivateOutpost:
					TryActivateOutpost();
					break;
				
				case OutpostPrompt.ViewLog:
					TryViewLog();
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
		
		void TryDishLever()
		{
			if (dishEnabled) return;

			OnOutpostUIUpdate?.Invoke(OutpostPrompt.DishLever, false);
			dishLeverAnim.SetBool("LeverEnabled", true);
		}

		// Called from lever animation
		public void HandleDishEnable()
		{
			if (dishEnabled) return;

			SoundManager.PlaySound(SoundType.LeverPull, 1.5f, false);
			dishEnabled = true;
		}

        void TryEnablePower()
		{
			if (outpostRepaired && dishEnabled && !isPowered)
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
			else
			{
				powerSwitchesAnim.SetTrigger("IsNotReady");
			}
			// Maybe notify player if not fully repaired
		}

		// Called from switch animation
		public void HandlePowerSwitchSound()
		{
			SoundManager.PlaySound(SoundType.SwitchFlip, 2f, false);
		}

		public void HandleOutpostStart()
		{
			SoundManager.PlaySound(SoundType.MachineStart, 2f, false);
			satelliteDishAnim.SetBool("IsPowered", true);
		}

		void TryActivateOutpost()
		{
			if (isPowered && !outpostActive)
			{
				outpostActive = true;
				bigButtonAnim.SetBool("IsPressed", true);
				OnOutpostUIUpdate?.Invoke(OutpostPrompt.ActivateOutpost, false);

				logCubeObj.SetActive(true);
				OnTriggerZoneActive?.Invoke(OutpostPrompt.ViewLog, true);
				OnOutpostUIUpdate?.Invoke(OutpostPrompt.ViewLog, true);
				OnCinematicSilhouetteSpawn?.Invoke();
				// Handle insanity start
				// Quest complete
			}
		}

		void DisableOutpost()
		{
			outpostActive = false;
			isPowered = false;

			OnOutpostHideAllUI?.Invoke();

			doorLight.material.SetColor("_BaseColor", Color.red);
			doorLight.material.SetColor("_EmissionColor", Color.red);

		}

		void TryViewLog()
		{
			Debug.Log("Trying to view outpost log");
			outpostUI.ShowLog();
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

// Player repairs Outpost exterior - done
// Power box can be used - done
// Player enables power - done
// Door becomes active and can be opened - done
// Player enters - done
// Outpost pressurizes and door closes - done
// Player can activate Outpost - done
// Player presses activate button - done
// Power flickers and sounds of powering up - done
// "Log downloaded" - voice clip starts to play - sorta done
// Player goes to exit and open door - done
// Door opens and a silhouette is seen for a short moment - done
// Cut to black and play insanity sounds. Silhouette is gone - done
// Insanity system starts 0 - 100%
// Return to base
