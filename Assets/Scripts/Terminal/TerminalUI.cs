using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.UI
{
	public class TerminalUI : MonoBehaviour
	{   
        [Header("References")]
        [SerializeField] TerminalController terminalController;
        [SerializeField] ProgressionManager progressionManager;

        [Header("Terminal Text")]
        [SerializeField] TMP_Text dateAndTime;

		[Header("Terminal Panels")]
        [SerializeField] GameObject terminalBGPanel;
		[SerializeField] GameObject terminalOpenText;
        [SerializeField] GameObject terminalInterfaceGroup;

        int lastSecond = -1;

        public string CurrentTime { get; private set; }

        // To BasePanel 
        public static event Action<PanelType> OnPanelSelected;
        public static event Action OnTerminalClosed;

		void OnEnable()
        {
            TerminalController.OnTerminalProximity += TerminalInteract;
            PlayerState.OnTerminalUIActive += TerminalPanel;
            ProgressionManager.OnStageChanged += StageUpdate;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalProximity -= TerminalInteract;
            PlayerState.OnTerminalUIActive -= TerminalPanel;
            ProgressionManager.OnStageChanged -= StageUpdate;
        }

        void Update()
        {
            DateAndTime();
            
            if(terminalController.terminalActive)
            {
                // DateAndTime();
            }
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
        void StageUpdate(ProgressionStage stage)
        {
            if (stage == ProgressionStage.Intro)
            {     
                //OnPanelSelected?.Invoke(PanelType.Intro);         
            }

            else if (stage == ProgressionStage.SampleObjective)
            {
                OnPanelSelected?.Invoke(PanelType.Interface); 
                //terminalInterfaceGroup.SetActive(true);
            }
        } 

        // public void HandleEnterLogScreenButton()
        // {
        //     if (terminalController.terminalActive)
        //     {
        //         OnPanelSelected?.Invoke(PanelType.Logs);
        //     }
        // }   

		void TerminalInteract(bool value)
        {
            terminalOpenText.SetActive(value);
        }
        
        void TerminalPanel(bool value)
        {
            if (value == true)
            {
                terminalBGPanel.SetActive(true);
                
                if (progressionManager.CurrentStage == ProgressionStage.Intro)
                {
                    OnPanelSelected?.Invoke(PanelType.Interface); // Hack
                }
                else// if (progressionManager.CurrentStage != ProgressionStage.Intro)
                {
                    OnPanelSelected?.Invoke(PanelType.Interface);
                }
            }
            else
            {
                //terminalInterfaceGroup.SetActive(false);
                terminalBGPanel.SetActive(false);
                OnTerminalClosed?.Invoke();
            }
        }
	}

    public enum PanelType
    {
        Intro,
        Interface,
        //Logs
    }
}
