using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace LunarAnomaly.UI
{
	public class TerminalUI : MonoBehaviour
	{   
        [Header("References")]
        [SerializeField] TerminalController terminalController;
        [SerializeField] ProgressionManager progressionManager;
        [SerializeField] NotificationController notificationController;

        [Header("Terminal Text")]
        [SerializeField] TMP_Text dateAndTime;

		[Header("Terminal Panels")]
		[SerializeField] GameObject terminalOpenText;
        [SerializeField] GameObject terminalWaitText;
        // [SerializeField] GameObject terminalBGPanel;
        //[SerializeField] GameObject terminalInterfaceGroup;

        int lastSecond = -1;

        bool introNotification;
        bool playerInProximity;

        public string CurrentTime { get; private set; }
        
        bool terminalActive;
        public bool TerminalActive => terminalActive;

        // To BasePanel
        public static Action<PanelType> OnPanelSelected; // Used in UIManager
        public static Action OnPanelClosed; // Used in OutpostUI and UIManager
        // To NotificationController - used in OutpostController, OutpostRevealCina
        public static Action<NotificationMessage> OnRequestNotification;
        public static Action<NotificationMessage, float> OnRequestNotificationDelayed;

		void OnEnable()
        {
            TerminalController.OnTerminalProximity += TerminalInteract;
            //UIManager.OnCursorUnlock += TerminalPanel; **** TEMP *****8
            UIManager.OnDisplayTerminal += DisplayTerminalPanel;
            // ProgressionManager.OnStageChanged += StageUpdate;
            TerminalInterfacePanel.OnIntroProceed += HandleIntroNotification;
            notificationController.OnWaitingForRevealChanged += UpdateTerminalEnterText;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalProximity -= TerminalInteract;
            //UIManager.OnCursorUnlock -= TerminalPanel;
            UIManager.OnDisplayTerminal -= DisplayTerminalPanel;
            // ProgressionManager.OnStageChanged -= StageUpdate;
            TerminalInterfacePanel.OnIntroProceed -= HandleIntroNotification;
            notificationController.OnWaitingForRevealChanged -= UpdateTerminalEnterText;
        }

        void Update()
        {
            // So that airlock logs show correct time
            DateAndTime();
            
            //if(terminalController.terminalWithinRange)
            //{
                // DateAndTime();
            //}
        }

        void DateAndTime()
        {              
            if (DateTime.Now.Second != lastSecond)
            {
                lastSecond = DateTime.Now.Second;                
                CurrentTime = DateTime.Now.ToString("HH:mm:ss");
                
                dateAndTime.text = CurrentTime;
            }
        }

        // This seems redundant
        // void StageUpdate(ProgressionStage stage)
        // {
        //     if (stage == ProgressionStage.Intro)
        //     {     
        //         //OnPanelSelected?.Invoke(PanelType.Intro);         
        //     }

        //     else if (stage == ProgressionStage.SampleObjective)
        //     {
        //         //OnPanelSelected?.Invoke(PanelType.Interface); 
        //         //terminalInterfaceGroup.SetActive(true);
        //     }
        // } 

		void TerminalInteract(bool value)
        {
            playerInProximity = value;
            UpdateTerminalEnterText();

            // if (value == true)
            // {
            //     if (notificationController.WaitingForReveal)
            //     {
            //         terminalWaitText.SetActive(true);    
            //         terminalOpenText.SetActive(false);                
            //     }
            //     else
            //     {
            //         terminalOpenText.SetActive(true);       
            //         terminalWaitText.SetActive(false);            
            //     }
            // }
            // else
            // {
                
            // }
        }

        void UpdateTerminalEnterText()
        {
            if (!playerInProximity)
            {
                terminalWaitText.SetActive(false);
                terminalOpenText.SetActive(false);
                return;
            }

            bool waiting = notificationController.WaitingForReveal;
            terminalWaitText.SetActive(waiting);
            terminalOpenText.SetActive(!waiting);  
        }
        
        void DisplayTerminalPanel(bool value)
        {
            if (notificationController.WaitingForReveal) return; 
            
            if (value == true)
            {
                terminalActive = true;
                OnPanelSelected?.Invoke(PanelType.Interface);
                // terminalBGPanel.SetActive(true);
                
                // if (progressionManager.CurrentStage == ProgressionStage.Intro)
                // {
                //     OnPanelSelected?.Invoke(PanelType.Interface); // Hack
                // }
                // else// if (progressionManager.CurrentStage != ProgressionStage.Intro)
                // {
                //     OnPanelSelected?.Invoke(PanelType.Interface);
                // }
            }
            else
            {
                terminalActive = false;
                //terminalInterfaceGroup.SetActive(false);
                // terminalBGPanel.SetActive(false);
                OnPanelClosed?.Invoke();
                
                if (introNotification)
                {
                    OnRequestNotification?.Invoke(NotificationMessage.Start1);
                    OnRequestNotificationDelayed?.Invoke(NotificationMessage.Start2, 10f);
                    introNotification = false;
                }
            }
        }

        void HandleIntroNotification()
        {
            introNotification = true;
        }
	}
}
