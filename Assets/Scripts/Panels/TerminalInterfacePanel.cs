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
		// 
		// [SerializeField] TMP_Text samplesCollectedText;
        [SerializeField] UpdateText terminalUpdateText;
		[SerializeField] TerminalController terminalController;
		[SerializeField] TerminalUI terminalUI;
		[SerializeField] TMP_Text currentTextBox;

        [Header("Panels")]
        [SerializeField] CanvasGroup notificationPanel;
        [SerializeField] CanvasGroup contentPanel;

        [Header("Terminal Logs")]
        [SerializeField] LogTextDatabase logTextDatabase;
        [SerializeField] TMP_Text headerTitle;
        [SerializeField] TMP_Text headerLabel;
        [SerializeField] TMP_Text headerDate;
        //[SerializeField] TMP_Text logText;
        [SerializeField] Button logButtonPrefab;
        [SerializeField] Transform logButtonContainer;

        [Header("Buttons")]
        [SerializeField] GameObject proceedButton;
        [SerializeField] GameObject depositButton;
        [SerializeField] Button notifButton;

        bool interfaceLocked;

        bool interfaceOpen;
        
        //int samplesDelivered;
        //bool sampleObjectiveComplete;		

        // To TerminalUpdateText
        public static event Action<LogMessage> OnLogMessage;
        // To ProgressionManager
        public static event Action OnPlayerProgressed;
        // To TerminalNotification
        public static event Action OnStartingAirlockNotification;
        public static event Action<bool> OnViewNotification;
        // To TerminalUI & HabitatController
        public static event Action OnIntroProceed;
        // To UpdateText
        public event Action OnDisableTerminalTextbox;

        protected override void OnEnable()
        {
            base.OnEnable();
            ProgressionManager.OnInterfaceLock += HandleInterfaceLock;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ProgressionManager.OnInterfaceLock -= HandleInterfaceLock;
        }

        protected override void OnPanelShown()
        {
            interfaceOpen = true;
            terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
			terminalController.RequestCurrentMessage();
            CreateLogButtons();
        }

        protected override void HidePanel()
        {
            base.HidePanel();
            
            if (interfaceOpen) 
            {
                OnDisableTerminalTextbox?.Invoke();
                interfaceOpen = false;
            }
        }

        public void HandleNotificationButton()
        {
            if (terminalUI.TerminalActive)
            {
                SoundManager.PlaySound(SoundType.MenuClick, 1, false);
                contentPanel.alpha = 0f;
                notificationPanel.alpha = 1f;

                headerLabel.text = "Notifications";
                headerTitle.text = "> System Notifications";
                headerDate.text = "Live";
                terminalController.RequestCurrentMessage();
                OnViewNotification?.Invoke(true);
            }
        }

        void HandleInterfaceLock(bool locked)
        {
            interfaceLocked = locked;

            UpdateNotifButton();
        }

        void UpdateNotifButton()
        {
            TextMeshProUGUI text = notifButton.GetComponentInChildren<TextMeshProUGUI>();

            if (!interfaceLocked)
            {
                notifButton.interactable = true;
                text.color = Color.white;
                CreateLogButtons();
            }
            else
            {
                notifButton.interactable = false;
                text.color = Color.darkSlateGray;
            }
        }

        public void HandleIntroProceedButton()
        {
            SoundManager.PlaySound(SoundType.MenuClick, 1, false);
            OnPlayerProgressed?.Invoke();
            OnStartingAirlockNotification?.Invoke();
            OnIntroProceed?.Invoke();
            proceedButton.SetActive(false);
            HandleNotificationButton();
        }


        // ******** MOVED TO HABITATCONTROLLER ********
		// void HandleDepositButton()
        // {
        //     if (terminalUI.TerminalActive) 
        //     {
        //         DepositSamples();
        //     }
        // }

		// void UpdateSamplesDeposited(int samples, int required)
        // {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            // samplesCollectedText.text = string.Format("Samples delivered: {0}/{1}", Mathf.Min(samples, required), required);
        // }

		// void DepositSamples()
        // {
        //     if (!terminalUI.TerminalActive) return;
        //     if (sampleObjectiveComplete) return;

        //     int samples = miningManager.samplesCollected;
            
        //     if (samples < 0) return;

        //     //AddDeliveredSamples(samples);
        //     miningManager.ClearSamples(); // <--- Consider not clearing all samples later
        // }

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
                string captureLogNumber = $"- Log {logIndex:D3}";
                text.text = captureLogNumber;
                logIndex++;

                //button.interactable = log.isDiscovered;
                button.gameObject.SetActive(log.isDiscovered);
                if (interfaceLocked) 
                {
                    button.interactable = false;
                    text.color = Color.darkSlateGray;
                }

                string capturedTitle = log.logTitle;
                string capturedDate = log.logDate;
                LogMessage capturedMessage = log.message;
                button.onClick.AddListener(() => OnLogButtonClicked(capturedMessage, capturedTitle, capturedDate, captureLogNumber));
            }

            void OnLogButtonClicked(LogMessage message, string title, string date, string number)
            {
                SoundManager.PlaySound(SoundType.MenuClick, 1, false);
                contentPanel.alpha = 1f;
                notificationPanel.alpha = 0f;

                headerLabel.text = number;
                headerTitle.text = $"> {title}";
                headerDate.text = date;
                OnLogMessage?.Invoke(message);

                OnViewNotification?.Invoke(false);
            }
        }
	}
}
