using System;
using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class DiscoveryZone : MonoBehaviour
    {
        public enum DiscoveryType { Habitat, Outpost, Anomaly }

        [SerializeField] DiscoveryType discoveryType;
        
        bool triggerActive;

        // To HabitatController
        public static event Action OnHabitatZoneEntered;        

        void OnTriggerEnter(Collider other)
        {       
            if (!triggerActive || !other.CompareTag("Player")) return;
            
            switch (discoveryType)
            {
                case DiscoveryType.Habitat:
                    TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.HabitatInRange);
                    OnHabitatZoneEntered?.Invoke();
                    HabitatController.OnUpdateWaypointActive?.Invoke(false);
                    triggerActive = false;
                    // Disable waypoint
                    break;
                
                case DiscoveryType.Outpost:
                    TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.OutpostObjective);
                    OutpostRepair.OnOutpostProgress?.Invoke(ProgressionStage.OutpostObjective);
                    HabitatController.OnUpdateWaypointActive?.Invoke(false);
                    triggerActive = false;
                    break;

                case DiscoveryType.Anomaly:
                    // Anomaly Logic
                    break;
            }              
        }

        public void ChangeActive(bool active)
        {
            triggerActive = active;
        }
    }
}