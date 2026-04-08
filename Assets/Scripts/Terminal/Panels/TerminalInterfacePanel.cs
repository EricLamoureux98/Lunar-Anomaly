using LunarAnomaly.Gameplay;
using TMPro;
using UnityEngine;

	
namespace LunarAnomaly.UI
{
	public class TerminalInterfacePanel : BasePanel
	{
        [Header("References")]
		[SerializeField] MiningManager miningManager;
		[SerializeField] TMP_Text samplesCollectedText;

		// [Header("Rock Samples")]
        // [SerializeField] int samplesRequired;
        
        int samplesDelivered;
        bool sampleObjectiveComplete;		

        protected override void OnPanelShown()
        {
            terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
			terminalController.RequestCurrentMessage();
        }

		public void HandleDepositButton()
        {
            if (terminalController.terminalActive) 
            {
                DepositSamples();
            }
        }

		void UpdateSamplesDeposited(int samples, int required)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples delivered: {0}/{1}", Mathf.Min(samples, required), required);
        }

		void DepositSamples()
        {
            if (!terminalController.terminalActive) return;
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
            UpdateSamplesDeposited(samplesDelivered, miningManager.samplesRequired);

            //Debug.Log($"Samples: {samplesDelivered} / {samplesRequired}");

            if (samplesDelivered >= miningManager.samplesRequired)
            {
				// ADD EVENT HERE
                //currentTerminalEntry = TerminalMessage.ObjectiveComplete;
                //OnTerminalMessage?.Invoke(currentTerminalEntry);
                //OnPlayerProgressed?.Invoke();
                sampleObjectiveComplete = true;
            }
            else
            {
                // Consider adding this. Needs updates to TerminalUI.ShowText
                // Samples received: {0}/{1}
                //string template = database.GetText(TerminalMessage.DepositInProgress);
                //string message = string.Format(template, samplesRemaining);

                //currentTerminalEntry = TerminalMessage.DepositSuccess;

                //OnTerminalMessage?.Invoke(currentTerminalEntry);
            }
        }
	}
}
