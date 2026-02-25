using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class AirlockSensor : MonoBehaviour
    {
        public enum SensorType {Interior, Exterior, Inside}

        [SerializeField] SensorType sensorType;

        Airlock airlock;

        void Awake()
        {
            airlock = GetComponentInParent<Airlock>();

            if (airlock == null) Debug.Log("airlock not found");
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            switch (sensorType)
            {
                case SensorType.Interior:
                    airlock.EnterFromInterior();
                    break;

                case SensorType.Exterior:
                    airlock.EnterFromExterior();
                    break;

                case SensorType.Inside:
                    airlock.PlayerInsideAirlock();
                    break;

                default:
                    Debug.Log("Unknown state");
                    break;
            }
        }
    }
}