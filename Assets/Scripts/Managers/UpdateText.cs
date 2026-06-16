using System.Collections.Generic;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.UI
{
	[RequireComponent(typeof(Typewriter))]
	public class UpdateText : MonoBehaviour
	{
		[Header("References")]
		[SerializeField] TerminalTextDatabase terminalTextDatabase;
        [SerializeField] LogTextDatabase logTextDatabase;
        [SerializeField] NotificationDatabase notificationDatabase;
		Typewriter typewriter;

		TMP_Text currentTextBox;

		// These are faster than a List
        HashSet<NotificationMessage> revealedNotificationMessage = new HashSet<NotificationMessage>();
		HashSet<TerminalMessage> revealedTerminalMessages = new HashSet<TerminalMessage>();
        HashSet<LogMessage> revealedLogMessages = new HashSet<LogMessage>();

        void OnEnable()
        {
            NotificationController.OnNotificationMessage += ReadText;
            TerminalController.OnTerminalMessage += ReadText;
            TerminalInterfacePanel.OnLogMessage += ReadText;
            TerminalUI.OnTerminalClosed += StopTypewriter;
            InputHandler.OnTextSpeedup += TextSpeedup;
        }

        void OnDisable()
        {
            NotificationController.OnNotificationMessage -= ReadText;
            TerminalController.OnTerminalMessage -= ReadText;
            TerminalInterfacePanel.OnLogMessage -= ReadText;
            TerminalUI.OnTerminalClosed -= StopTypewriter;
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

        void ReadText(TerminalMessage terminalMessage)
        {
            string text = terminalTextDatabase.GetText(terminalMessage);

            ShowText(text, terminalMessage, revealedTerminalMessages);
        }

        void ReadText(LogMessage logMessage)
        {
            string text = logTextDatabase.GetLogText(logMessage);
            
            ShowText(text, logMessage, revealedLogMessages);
        }

        void ReadText(NotificationMessage notificationMessage)
        {
            string text = notificationDatabase.GetNotificationText(notificationMessage);

            ShowText(text, notificationMessage, revealedNotificationMessage);
        }

        // This is a generic method
        void ShowText<T>(string text, T message, HashSet<T> revealedMessages)
        {
            if (typewriter == null) return;
            
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

        public void ShowWithTypewriter(string text)
        {
            typewriter.SetText(text, currentTextBox);
        }

        void StopTypewriter()
        {
            typewriter.Stop();
        }

		void TextSpeedup()
        {
            // Might not use
            //typewriter.Skip();
        }
	}
}
