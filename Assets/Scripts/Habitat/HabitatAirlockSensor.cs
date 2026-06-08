using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class HabitatAirlockSensor : MonoBehaviour
    {
        public enum SensorType {Interior, Exterior, Inside}

        [SerializeField] SensorType sensorType;

        HabitatAirlock airlock;

        void Awake()
        {
            airlock = GetComponentInParent<HabitatAirlock>();

            if (airlock == null) Debug.Log("airlock not found");
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            switch (sensorType)
            {
                case SensorType.Interior:
                    //airlock.EnterFromInterior();
                    break;

                case SensorType.Exterior:
                    //airlock.EnterFromExterior();
                    break;

                case SensorType.Inside:
                    airlock.PlayerInsideAirlock();
                    break;

                default:
                    Debug.Log("Unknown state");
                    break;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (sensorType == SensorType.Inside)
            {
                airlock.PlayerExitedAirlock();
            }
        }
    }
}