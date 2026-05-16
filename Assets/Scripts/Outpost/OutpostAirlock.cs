using System.Collections;
using LunarAnomaly.UI;
using UnityEngine;
	
namespace LunarAnomaly.Gameplay
{
	public class OutpostAirlock : MonoBehaviour
	{
		[SerializeField] LightFlicker lightFlicker;
		AtmosphereZone atmosphereZone;

		[Header("Airlock")]
		[SerializeField] Animator OutpostDoorAnim;
		[SerializeField] float pressurizationTime = 3f;
		bool playerInside = false;
		bool isCycling = false;
		
		Coroutine cyclingRoutine;

		void Awake()
        {
			atmosphereZone = GetComponent<AtmosphereZone>();
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
   
			OutpostController.OnOutpostAdvanced?.Invoke(ProgressionStage.OutpostObjective);
			playerInside = false;
			
			// if (cyclingRoutine != null)
			// {
			// 	StopCoroutine(cyclingRoutine);
			// 	cyclingRoutine = null;
			// }
        }

		public void HandleDoorOpen(bool tryOpen)
		{
			if (tryOpen && !isCycling)
			{
				OutpostDoorAnim.SetBool("IsOpen", true);
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
			SoundManager.PlaySound(SoundType.Alarm, 1.25f, false);
			SoundManager.PlaySound(fromExterior ? SoundType.GainAtmosphere : SoundType.LoseAtmosphere, 1f, false);
			lightFlicker.StartFlicker(3f);
			
			yield return new WaitForSeconds(pressurizationTime);
			
			atmosphereZone.SetPressuized(fromExterior);
			Airlock.OnEnterAtmosphere?.Invoke(fromExterior);			
			
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
