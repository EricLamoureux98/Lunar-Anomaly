using System;
using LunarAnomaly.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LunarAnomaly.UI
{
	public class TerminalInterfacePanel : BasePanel
	{
        [Header("References")]
		[SerializeField] MiningManager miningManager;
		[SerializeField] TMP_Text samplesCollectedText;

        [Header("Terminal Logs")]
        [SerializeField] LogTextDatabase logTextDatabase;
        [SerializeField] TMP_Text logTitle;
        [SerializeField] TMP_Text logDate;
        //[SerializeField] TMP_Text logText;
        [SerializeField] Button logButtonPrefab;
        [SerializeField] Transform logButtonContainer;

		// [Header("Rock Samples")]
        // [SerializeField] int samplesRequired;
        
        int samplesDelivered;
        bool sampleObjectiveComplete;		

        public static event Action<LogMessage> OnLogMessage;

        protected override void OnPanelShown()
        {
            terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
			terminalController.RequestCurrentMessage();
            CreateLogButtons();
        }

        public void HandleNotificationButton()
        {
            if (terminalController.terminalActive)
            {
                logTitle.text = "Notifications";
                logDate.text = "Live";
                terminalController.RequestCurrentMessage();
            }
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

            //AddDeliveredSamples(samples);
            miningManager.ClearSamples(); // <--- Consider not clearing all samples later
        }

        // void AddDeliveredSamples(int amount)
        // {
        //     Debug.Log("Trying to deposit samples");
        //     samplesDelivered += amount;
            
        //     // MOVING TO ObjectiveManager 
        //     //UpdateSamplesDeposited(samplesDelivered, miningManager.samplesRequired);

        //     // MOVING TO ObjectiveManager 
        //     if (samplesDelivered >= miningManager.samplesRequired)
        //     {
		// 		// ADD EVENT HERE
        //         //currentTerminalEntry = TerminalMessage.ObjectiveComplete;
        //         //OnTerminalMessage?.Invoke(currentTerminalEntry);
        //         //OnPlayerProgressed?.Invoke();
        //         sampleObjectiveComplete = true;
        //     }
        //     else
        //     {
        //         // Consider adding this. Needs updates to TerminalUI.ShowText
        //         // Samples received: {0}/{1}
        //         //string template = database.GetText(TerminalMessage.DepositInProgress);
        //         //string message = string.Format(template, samplesRemaining);

        //         //currentTerminalEntry = TerminalMessage.DepositSuccess;

        //         //OnTerminalMessage?.Invoke(currentTerminalEntry);
        //     }
        // }

        void CreateLogButtons()
        {
            // Makes sure spawned buttons update with isDiscoverd
            foreach (Transform child in logButtonContainer)
                Destroy(child.gameObject);

            int logIndex = 1;
            foreach (var log in logTextDatabase.logEntries)
            {
                Button button = Instantiate(logButtonPrefab, logButtonContainer);
                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
                //text.text = log.logTitle;
                text.text = $"- Log {logIndex:D3}";
                logIndex++;

                //button.interactable = log.isDiscovered;
                button.gameObject.SetActive(log.isDiscovered);

                string capturedTitle = log.logTitle;
                string capturedDate = log.logDate;
                LogMessage capturedMessage = log.message;
                button.onClick.AddListener(() => OnLogButtonClicked(capturedMessage, capturedTitle, capturedDate));
            }

            void OnLogButtonClicked(LogMessage message, string title, string date)
            {
                // safe to remove later
                //logText.text = logTextDatabase.GetLogText(message);

                logTitle.text = title;
                logDate.text = date;
                OnLogMessage?.Invoke(message);
            }
        }
	}
}
