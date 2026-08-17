using System.Collections;
using LunarAnomaly.UI;
using UnityEngine;
	
namespace LunarAnomaly.Gameplay
{
	public class OutpostAirlock : MonoBehaviour
	{
		[SerializeField] LightFlicker lightFlicker;
		AtmosphereZone atmosphereZone;
		OutpostController outpostController;

		[Header("Airlock")]
		[SerializeField] Animator OutpostDoorAnim;
		[SerializeField] float pressurizationTime = 3f;
		bool playerInside = false;
		bool isCycling = false;
		
		Coroutine cyclingRoutine;

		void Awake()
        {
			atmosphereZone = GetComponent<AtmosphereZone>();
			outpostController = GetComponentInParent<OutpostController>();
        }

		void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
			
			OutpostController.OnOutpostAdvanced?.Invoke(ProgressionStage.OutpostObjective);
			OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.EnterOutpost, false);
			playerInside = true;
			cyclingRoutine = StartCoroutine(CycleAtmosphere(true));
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
   
			// --- OUTPOST COMPLETE ---
			//if (outpostController.outpostActive)
				//OutpostController.OnOutpostAdvanced?.Invoke(ProgressionStage.OutpostObjective);
				
			playerInside = false;
			
			// if (cyclingRoutine != null)
			// {
			// 	StopCoroutine(cyclingRoutine);
			// 	cyclingRoutine = null;
			// }
        }

		public void HandleDoorOpen(bool tryOpen)
		{
			OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.EnterOutpost, !tryOpen);
			OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.ExitOutpost, !tryOpen);
			OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.EnterOutpost, !tryOpen);
			OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.ExitOutpost, !tryOpen);

			if (tryOpen && !isCycling)
			{
				OutpostDoorAnim.SetBool("IsOpen", true);
				// OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.EnterOutpost, false);
				// OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.ExitOutpost, false);
				// OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.EnterOutpost, false);
				// OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.ExitOutpost, false);
				return;
			}
			else if (!tryOpen)
			{
				OutpostDoorAnim.SetBool("IsOpen", false);
				return;
			}
		}

		public void TryExitOutpost() => TryCycle(fromExterior: false);

		void TryCycle(bool fromExterior)
		{
			if (isCycling) return;
			
			StartCoroutine(CycleAtmosphere(fromExterior));
		}

		IEnumerator CycleAtmosphere(bool fromExterior)
		{
			isCycling = true;
			
			yield return new WaitForSeconds(1);
			
			if (!playerInside) yield break;

			HandleDoorOpen(false);
			SoundManager.PlaySound(SoundType.Alarm, 1.25f);
			SoundManager.PlaySound(fromExterior ? SoundType.GainAtmosphere : SoundType.LoseAtmosphere, 1f);
			lightFlicker.StartFlicker(3f);
			
			yield return new WaitForSeconds(pressurizationTime);
			
			atmosphereZone.SetPressuized(fromExterior);
			HabitatAirlock.OnEnterAtmosphere?.Invoke(fromExterior);			
			
			isCycling = false;
			
			if (!fromExterior) 
			{
				HandleDoorOpen(true);
			}

			//OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.ExitOutpost, fromExterior);
			//OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.ExitOutpost, fromExterior);
			
			yield return new WaitUntil(() => !playerInside);
			yield return new WaitForSeconds(1f);

			HandleDoorOpen(false);
			//SoundManager.PlaySound(SoundType.Airlock, 0.5f);
			//OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.EnterOutpost, true);
			
		}
	}
}
