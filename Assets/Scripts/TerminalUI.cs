using System.Collections.Generic;
using LunarAnomaly.Gameplay;
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

		[Header("Terminal UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;
        [SerializeField] GameObject terminalUIPanel;
		[SerializeField] GameObject terminalOpenText;
        //[SerializeField] TextMeshProUGUI dialogTextField;

        HashSet<TerminalMessage> revealedMessages = new HashSet<TerminalMessage>();

		void OnEnable()
        {
            TerminalController.OnTerminalProximity += TerminalInteract;
            PlayerState.OnTerminalUIActive += TerminalPanel;
            TerminalController.OnTerminalDeposit += UpdateSamplesDeposited;
            TerminalController.OnTerminalMessage += ShowText;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalProximity -= TerminalInteract;
            PlayerState.OnTerminalUIActive -= TerminalPanel;
            TerminalController.OnTerminalDeposit -= UpdateSamplesDeposited;
            TerminalController.OnTerminalMessage -= ShowText;
        }

        // void ShowText(TerminalMessage message)
        // {
        //     if (typewriter == null) return;

        //     string text = database.GetText(message);
        //     typewriter.SetText(text);
        // }
        
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
            typewriter.SetTextInstant(text);
        }

        void ShowWithTypewriter(string text)
        {
            typewriter.SetText(text);
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
            terminalUIPanel.SetActive(value);        

            if (value)
            {
                terminalController.RequestCurrentMessage();
            }
        }
	}
}
