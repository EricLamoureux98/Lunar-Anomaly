using LunarAnomaly.Gameplay;
using UnityEngine;

	
namespace LunarAnomaly
{
	public class OutpostAirlockSensor : MonoBehaviour
	{
		OutpostAirlock outpostAirlock;

		void Awake()
        {
            outpostAirlock = GetComponentInParent<OutpostAirlock>();

            if (outpostAirlock == null) Debug.Log("airlock not found");
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			
        }
    }
}
