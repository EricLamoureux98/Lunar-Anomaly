using LunarAnomaly.UI;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class SolarPanel : MonoBehaviour
    {
        [SerializeField] Cable cable;

        bool cableConnected;

        void OnEnable() => OutpostTriggerZone.OnInteract += HandleInteract;
        void OnDisable() => OutpostTriggerZone.OnInteract -= HandleInteract;

        void HandleInteract(OutpostPrompt prompt)
        {
            switch (prompt)
            {
                case OutpostPrompt.PickupCable:
                    PickupCable();
                    break;

                case OutpostPrompt.ConnectCable:
                    ConnectCable();
                    break;
            }
        }

        void PickupCable()
        {
            OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.PickupCable, false);
            OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.PickupCable, false);

            OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.ConnectCable, true);
            OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.ConnectCable, true);
        
            cable.SetActive(true);
        }

        void ConnectCable()
        {
            if (cableConnected) return; 

            SoundManager.PlaySound(SoundType.Electricity, 1.5f);

            cableConnected = true;

            OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.ConnectCable, false);
            OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.ConnectCable, false);

            cable.SetLastPosition();
            cable.SetActive(false);

            OutpostRepair.OnOutpostProgress?.Invoke(ProgressionStage.OutpostObjective);
        }
    }
}

