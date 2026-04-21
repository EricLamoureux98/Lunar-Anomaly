using System.Collections;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostAirlock : MonoBehaviour
	{
		AtmosphereZone atmosphereZone;

		[Header("Airlock")]
		[SerializeField] float pressurizationTime = 3f;
		bool playerInside = false;
		bool isCycling = false;

		void Awake()
        {
			atmosphereZone = GetComponentInChildren<AtmosphereZone>();
        }

		void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			playerInside = true;
			Debug.Log("Player entered outpost");
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			playerInside = false;
			Debug.Log("Player exited outpost");
        }

        void WaitForPlayerEnter()
		{
			if (playerInside)
			{
				if (isCycling) return;

				StartCoroutine(CycleAtmosphere());
			}
		}

		IEnumerator CycleAtmosphere()
		{
			yield return new WaitForSeconds(1);
		}
	}
}
