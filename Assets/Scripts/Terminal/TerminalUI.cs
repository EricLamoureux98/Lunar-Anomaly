using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using UnityEngine;

namespace LunarAnomaly.UI
{
	public class TerminalUI : MonoBehaviour
	{   
        [Header("References")]
        [SerializeField] TerminalController terminalController;
        [SerializeField] ProgressionManager progressionManager;

		[Header("Terminal UI")]
        [SerializeField] GameObject terminalBGPanel;
		[SerializeField] GameObject terminalOpenText;

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

        // This seems redundant
        void StageUpdate(ProgressionStage stage)
        {
            if (stage == ProgressionStage.Intro)
            {
                //TerminalIntro();       
                OnPanelSelected?.Invoke(PanelType.Intro);         
            }

            else if (stage == ProgressionStage.SampleObjective)
            {
                //TerminalInterface();
                OnPanelSelected?.Invoke(PanelType.Interface);   
            }
        } 

        public void HandleEnterLogScreenButton()
        {
            if (terminalController.terminalActive)
            {
                OnPanelSelected?.Invoke(PanelType.Logs);
            }
        }   

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
                    OnPanelSelected?.Invoke(PanelType.Intro);
                }
                else if (progressionManager.CurrentStage == ProgressionStage.SampleObjective)
                {  
                    OnPanelSelected?.Invoke(PanelType.Interface);                  
                }
            }
            else
            {
                terminalBGPanel.SetActive(false);
                OnTerminalClosed?.Invoke();
            }
        }
	}

    public enum PanelType
    {
        Intro,
        Interface,
        Logs
    }
}
