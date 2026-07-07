using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class OutpostDiscoveryZone : MonoBehaviour
    {
        bool triggerActive;

        public void ChangeActive(bool active)
        {
            triggerActive = active;
        }

        void OnTriggerEnter(Collider other)
        {       
            if (other.CompareTag("Player") && triggerActive)
            {
                // Debug.Log("Player discovered outpost");
                TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.OutpostObjective);
                OutpostRepair.OnOutpostProgress?.Invoke(ProgressionStage.OutpostObjective);
                HabitatController.OnUpdateWaypointActive?.Invoke(false);
                triggerActive = false;
            }
        }
    }
}

