using System;
using System.Collections;
using LunarAnomaly.Gameplay;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.UI
{
    public class NotificationController : MonoBehaviour
    {
        [SerializeField] UpdateText updateText;
        [SerializeField] TMP_Text currentTextBox;
        [SerializeField] Typewriter typewriter;
        [SerializeField] CanvasGroup canvasGroup;

        [SerializeField] float hideDelay;

        // To UpdateText
        public static event Action<NotificationMessage> OnNotificationMessage;

        void OnEnable()
        {
            TerminalUI.OnRequestNotification += RequestNotification;
            Typewriter.OnCompleteTextRevealed += RequestHideNotification;
        }

        void OnDisable()
        {
            TerminalUI.OnRequestNotification -= RequestNotification;
            Typewriter.OnCompleteTextRevealed -= RequestHideNotification;
        }

        void RequestNotification(NotificationMessage message)
        {
            updateText.UpdateCurrentTextBox(currentTextBox);
            canvasGroup.alpha = 1f;
            OnNotificationMessage?.Invoke(message);
        }

        void RequestHideNotification()
        {
            Invoke("HideNotification", hideDelay);
        }

        void HideNotification()
        {
            canvasGroup.alpha = 0f;
        }
    }
}

