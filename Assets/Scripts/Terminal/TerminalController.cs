using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class TerminalController : MonoBehaviour
    {
        [SerializeField] LayerMask playerLayer;
        [SerializeField] Animator anim;

        TerminalMessage currentTerminalEntry;

        // To TerminalUI and UIManager
        public static event Action<bool> OnTerminalProximity;
        // To TerminalUpdateText
        public static event Action<TerminalMessage> OnTerminalMessage;

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
            //currentTerminalEntry = TerminalMessage.Intro;  
        }

        void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            OnTerminalProximity?.Invoke(true);
            anim.SetBool("isActive", true);
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            OnTerminalProximity?.Invoke(false);
            anim.SetBool("isActive", false);
        }

        public void RequestCurrentMessage()
        {
            OnTerminalMessage?.Invoke(currentTerminalEntry);
        }

        void SetupMiningSample()
        {
            currentTerminalEntry = TerminalMessage.Greeting;
        }

        // Probably don't need this anymore
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
                
                case ProgressionStage.OutpostObjective:
                    break;
                
                case ProgressionStage.Outro:
                    break;
            }
        }

        // void OnDrawGizmosSelected()
        // {
        //     SphereCollider terminalCollider = GetComponent<SphereCollider>();
        //     if (terminalCollider == null) return;

        //     float worldRadius = terminalCollider.radius * terminalCollider.transform.lossyScale.x;

        //     Gizmos.color = terminalActive ? Color.green : Color.red;

        //     Gizmos.DrawWireSphere(terminalCollider.transform.position, worldRadius);
        // }
    }
}
