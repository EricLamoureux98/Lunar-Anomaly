using System.Collections.Generic;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using LunarAnomaly.Player;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.UI
{
	public class TerminalUI : MonoBehaviour
	{   
        [Header("References")]
        [SerializeField] TerminalTextDatabase database;
        [SerializeField] Typewriter typewriter;
        [SerializeField] TerminalController terminalController;
        [SerializeField] ProgressionManager PM;

		[Header("Terminal UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;
        [SerializeField] GameObject terminalBGPanel;
        [SerializeField] GameObject terminalInterfacePanel;
		[SerializeField] GameObject terminalOpenText;
        [SerializeField] TMP_Text interfaceText;

        [Header("Terminal Intro")]
        [SerializeField] GameObject terminalIntroPanel;
        [SerializeField] TMP_Text introText;

        TMP_Text currentTextBox;

        HashSet<TerminalMessage> revealedMessages = new HashSet<TerminalMessage>();

		void OnEnable()
        {
            TerminalController.OnTerminalProximity += TerminalInteract;
            PlayerState.OnTerminalUIActive += TerminalPanel;
            TerminalController.OnTerminalDeposit += UpdateSamplesDeposited;
            TerminalController.OnTerminalMessage += ShowText;
            ProgressionManager.OnStageChanged += StageUpdate;
            InputHandler.OnTextSpeedup += TextSpeedup;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalProximity -= TerminalInteract;
            PlayerState.OnTerminalUIActive -= TerminalPanel;
            TerminalController.OnTerminalDeposit -= UpdateSamplesDeposited;
            TerminalController.OnTerminalMessage -= ShowText;
            ProgressionManager.OnStageChanged -= StageUpdate;
            InputHandler.OnTextSpeedup -= TextSpeedup;
        }

        void Start()
        {
            currentTextBox = introText;
        }

        void ShowText(TerminalMessage message)
        {
            if (typewriter == null) return;

            string text = database.GetText(message);
            
            if (revealedMessages.Contains(message))
            {
                ShowInstantText(text);
            }
            else
            {
                ShowWithTypewriter(text);
                revealedMessages.Add(message);
            }
        }

        void ShowInstantText(string text)
        {
            typewriter.SetTextInstant(text, currentTextBox);
        }

        void ShowWithTypewriter(string text)
        {
            typewriter.SetText(text, currentTextBox);
        }

        void StageUpdate(ProgressionStage stage)
        {
            if (stage == ProgressionStage.Intro)
            {
                TerminalIntro();                
            }

            else if (stage == ProgressionStage.SampleObjective)
            {
                TerminalInterface();
            }
        }

        void TextSpeedup()
        {
            // Might not use
            //typewriter.Skip();
        }

        void TerminalIntro()
        {
            currentTextBox = introText;
            terminalInterfacePanel.SetActive(false); 
            terminalBGPanel.SetActive(true);  
            terminalIntroPanel.SetActive(true);    
            terminalController.RequestCurrentMessage();
        }

        void TerminalInterface()
        {
            currentTextBox = interfaceText;
            terminalIntroPanel.SetActive(false);  
            terminalInterfacePanel.SetActive(true); 
            terminalBGPanel.SetActive(true);
            terminalController.RequestCurrentMessage();
        }

        void UpdateSamplesDeposited(int samples, int required)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples delivered: {0}/{1}", Mathf.Min(samples, required), required);
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
                if (PM.CurrentStage == ProgressionStage.Intro)
                {
                    TerminalIntro();
                }
                else if (PM.CurrentStage == ProgressionStage.SampleObjective)
                {
                    TerminalInterface();                        
                }
            }
            else
            {
                terminalBGPanel.SetActive(false);
                terminalIntroPanel.SetActive(false);  
                terminalInterfacePanel.SetActive(false); 
            }
        }
	}
}
