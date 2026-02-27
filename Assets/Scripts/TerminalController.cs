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
        
        // public for testing
        public int samplesDelivered;
        public bool sampleObjectiveComplete;
        public bool terminalActive; 

        // To TerminalUI and PlayerState
        public static event Action<bool> OnTerminalProximity;
        public static event Action<int, int> OnTerminalDeposit;

        void Start()
        {
            AddDeliveredSamples(0);
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

        public void HandleDepositButton()
        {
            //if (terminalActive && playerDepositing && !wasDepositingLastFrame) 
            if (terminalActive) 
            {
                DepositSamples();
            }
        }

        void DepositSamples()
        {
            if (!terminalActive) return;
            if (sampleObjectiveComplete) return;

            int samples = miningManager.samplesCollected;
            
            if (samples <= 0) return;

            AddDeliveredSamples(samples);
            miningManager.ClearSamples(); // <--- Consider not clearing all samples later
        }

        void AddDeliveredSamples(int amount)
        {
            samplesDelivered += amount;
            OnTerminalDeposit?.Invoke(samplesDelivered, samplesRequired);

            //Debug.Log($"Samples: {samplesDelivered} / {samplesRequired}");

            if (samplesDelivered >= samplesRequired)
            {
                CompleteObjectives();
                // Notify terminal UI
            }
        }

        void CompleteObjectives()
        {
            sampleObjectiveComplete = true;
            Debug.Log("Sample objective complete!");
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
