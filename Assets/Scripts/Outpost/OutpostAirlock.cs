using System.Collections;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostAirlock : MonoBehaviour
	{
		AtmosphereZone atmosphereZone;

		[Header("Airlock")]
		[SerializeField] Animator OutpostDoorAnim;
		[SerializeField] float pressurizationTime = 3f;
		bool playerInside = false;
		bool isCycling = false;

		bool doorOpen;

		void Awake()
        {
			atmosphereZone = GetComponent<AtmosphereZone>();
        }

		void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
			
			OutpostController.OnTriggerZoneActive?.Invoke(UI.OutpostPrompt.EnterOutpost, false);
			playerInside = true;
			StartCoroutine(CycleAtmosphere());
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			playerInside = false;
        }

		public void HandleDoorOpen(bool isOpen)
		{
			//doorOpen = isOpen;
			if (isOpen && !isCycling)
			{
				OutpostDoorAnim.SetBool("IsOpen", true);
				return;
			}
			else if (!isOpen)
			{
				OutpostDoorAnim.SetBool("IsOpen", false);
				return;
			}
			
			
			
		}

		IEnumerator CycleAtmosphere()
		{
			isCycling = true;

			yield return new WaitForSeconds(1);

			HandleDoorOpen(false);
		}
	}
}
