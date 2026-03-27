using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class TerminalController : MonoBehaviour
    {
        [SerializeField] MiningManager miningManager;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] Animator anim;

        [Header("Rock Samples")]
        [SerializeField] int samplesRequired;
        
        int samplesDelivered;
        bool sampleObjectiveComplete;
        bool terminalActive; 
        TerminalMessage currentTerminalEntry;
        //ProgressionStage progressionStage;

        // To TerminalUI and PlayerState
        public static event Action<bool> OnTerminalProximity;
        public static event Action<int, int> OnTerminalDeposit;
        public static event Action<TerminalMessage> OnTerminalMessage;
        // To ProgressionManager
        public static event Action OnPlayerProgressed;

        void OnEnable()
        {
            ProgressionManager.OnStageChanged += UpdateStage;
        }

        void OnDisable()
        {
            ProgressionManager.OnStageChanged -= UpdateStage;
        }

        void Start()
        {
            currentTerminalEntry = TerminalMessage.Intro;  
            //UpdateStage(ProgressionStage.Intro);
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            OnTerminalProximity?.Invoke(true);
            terminalActive = true;
            anim.SetBool("isActive", true);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            OnTerminalProximity?.Invoke(false);
            terminalActive = false;
            anim.SetBool("isActive", false);
        }

        public void RequestCurrentMessage()
        {
            OnTerminalMessage?.Invoke(currentTerminalEntry);
        }
        
        public void HandleIntroProceed()
        {
            if (terminalActive)
            {
                UpdateStage(ProgressionStage.SampleObjective);
                OnPlayerProgressed?.Invoke();
            }
        }

        public void HandleDepositButton()
        {
            if (terminalActive) 
            {
                DepositSamples();
            }
        }

        void SetupMiningSample()
        {
            //DepositSamples();
            // Show Sample UI in TerminalUI
            currentTerminalEntry = TerminalMessage.Greeting;
        }

        void DepositSamples()
        {
            if (!terminalActive) return;
            if (sampleObjectiveComplete) return;

            int samples = miningManager.samplesCollected;
            
            if (samples < 0) return;

            AddDeliveredSamples(samples);
            miningManager.ClearSamples(); // <--- Consider not clearing all samples later
        }

        void AddDeliveredSamples(int amount)
        {
            Debug.Log("Trying to deposit samples");
            samplesDelivered += amount;
            OnTerminalDeposit?.Invoke(samplesDelivered, samplesRequired);

            //Debug.Log($"Samples: {samplesDelivered} / {samplesRequired}");

            if (samplesDelivered >= samplesRequired)
            {
                currentTerminalEntry = TerminalMessage.ObjectiveComplete;
                OnTerminalMessage?.Invoke(currentTerminalEntry);
                OnPlayerProgressed?.Invoke();
                sampleObjectiveComplete = true;
            }
            else
            {
                // Consider adding this. Needs updates to TerminalUI.ShowText
                // Samples received: {0}/{1}
                //string template = database.GetText(TerminalMessage.DepositInProgress);
                //string message = string.Format(template, samplesRemaining);

                currentTerminalEntry = TerminalMessage.DepositSuccess;

                OnTerminalMessage?.Invoke(currentTerminalEntry);
            }
        }

        void UpdateStage(ProgressionStage newStage)
        {
            switch (newStage)
            {
                case ProgressionStage.Intro:
                    currentTerminalEntry = TerminalMessage.Intro;
                    break;
                
                case ProgressionStage.SampleObjective:
                    SetupMiningSample();
                    break;
                
                case ProgressionStage.RepairObjective:
                    break;
                
                case ProgressionStage.Outro:
                    break;
            }

        }

        void OnDrawGizmosSelected()
        {
            SphereCollider terminalCollider = GetComponent<SphereCollider>();
            if (terminalCollider == null) return;

            float worldRadius = terminalCollider.radius * terminalCollider.transform.lossyScale.x;

            Gizmos.color = terminalActive ? Color.green : Color.red;

            Gizmos.DrawWireSphere(terminalCollider.transform.position, worldRadius);
        }
    }
}
