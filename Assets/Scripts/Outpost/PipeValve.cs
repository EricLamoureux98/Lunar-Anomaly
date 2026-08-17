using LunarAnomaly.UI;
using Unity.Cinemachine;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class PipeValve : MonoBehaviour
    {
        [SerializeField] CinemachineImpulseSource outpostImpulseSource;
        
        [SerializeField] GameObject brokenPipe, repairedPipe;

        [SerializeField] GameObject brokenValve;
        [SerializeField] GameObject playerHoldingValve;

        [SerializeField] GameObject missingValveTxt;
        [SerializeField] GameObject repairValveTxt;

        [SerializeField] Animator valveAnim;

        bool valveInHand;

        void OnEnable() => OutpostTriggerZone.OnInteract += HandleInteract;
        void OnDisable() => OutpostTriggerZone.OnInteract -= HandleInteract;

        void HandleInteract(OutpostPrompt prompt)
        {
            switch (prompt)
            {
                case OutpostPrompt.PickupValve:
                    PickupValve();
                    break;

                case OutpostPrompt.ConnectValve:
                    AttachValve();
                    break;

                case OutpostPrompt.TurnValve:
                    TurnValve();
                    break;
            }
        }

        void PickupValve()
        {
            if (valveInHand) return;

            SoundManager.PlaySoundWithVariation(SoundType.Pickup, 0.5f);
            missingValveTxt.SetActive(false);
            repairValveTxt.SetActive(true);

            valveInHand = true;
            Destroy(brokenValve);
            playerHoldingValve.SetActive(true);
        }

        void AttachValve()
        {
            if (!valveInHand) return; 

            SoundManager.PlaySoundWithVariation(SoundType.MetalSlam, 1f);
            CameraShakeManager.Instance.CameraShake(outpostImpulseSource, 0.03f);
            
            Destroy(playerHoldingValve);
            brokenPipe.SetActive(false);
            repairedPipe.SetActive(true);
        }

        void TurnValve()
        {
            SoundManager.PlaySound(SoundType.OutpostValve, 0.65f);
            valveAnim.SetBool("isTurning", true);
            OutpostController.OnTriggerZoneActive?.Invoke(OutpostPrompt.TurnValve, false);
            OutpostController.OnOutpostUIUpdate?.Invoke(OutpostPrompt.TurnValve, false);
        }

        // Called from animation
        public void NotifyValveRepaired()
        {
            OutpostRepair.OnOutpostProgress?.Invoke(ProgressionStage.OutpostObjective);
        }
    }
}

