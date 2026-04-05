using System.Collections.Generic;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.UI
{
	[RequireComponent(typeof(Typewriter))]
	public class TerminalUpdateText : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] TerminalTextDatabase terminalTextDatabase;
		Typewriter typewriter;

		TMP_Text currentTextBox;

		// These are faster than a List
		HashSet<TerminalMessage> revealedMessages = new HashSet<TerminalMessage>();
        HashSet<LogMessage> foundLogMessages = new HashSet<LogMessage>();

        void OnEnable()
        {
            TerminalController.OnTerminalMessage += ShowText;
            InputHandler.OnTextSpeedup += TextSpeedup;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalMessage -= ShowText;
            InputHandler.OnTextSpeedup -= TextSpeedup;
        }

        void Awake()
        {
            typewriter = GetComponent<Typewriter>();
        }

        public void UpdateCurrentTextBox(TMP_Text textBox)
        {
            if (textBox == currentTextBox) return;

            currentTextBox = textBox;
        }

        public void ShowText(TerminalMessage message)
        {
            if (typewriter == null) return;

            string text = terminalTextDatabase.GetText(message);
            
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

		void TextSpeedup()
        {
            // Might not use
            //typewriter.Skip();
        }
	}
}
