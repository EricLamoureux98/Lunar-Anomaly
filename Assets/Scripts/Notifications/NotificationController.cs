using System;
using System.Collections;
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
        [SerializeField] TerminalUI terminalUI;

        [SerializeField] float hideDelay;

        Coroutine delayedNotificationRoutine;

        // To UpdateText
        public static event Action<NotificationMessage> OnNotificationMessage;

        void OnEnable()
        {
            TerminalUI.OnRequestNotification += RequestNotification;
            TerminalUI.OnRequestNotificationDelayed += RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed += RequestHideNotification;
        }

        void OnDisable()
        {
            TerminalUI.OnRequestNotification -= RequestNotification;
            TerminalUI.OnRequestNotificationDelayed -= RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed -= RequestHideNotification;
        }

        void RequestNotification(NotificationMessage message)
        {
            updateText.UpdateCurrentTextBox(currentTextBox);
            canvasGroup.alpha = 1f;
            OnNotificationMessage?.Invoke(message);
        }

        void RequestNotificationDelayed(NotificationMessage message, float delay)
        {
            if (delayedNotificationRoutine != null) return;
            delayedNotificationRoutine = StartCoroutine(DelayedNotification(message, delay));
        }

        IEnumerator DelayedNotification(NotificationMessage message, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => !terminalUI.TerminalActive);

            CancelInvoke(nameof(HideNotification));

            updateText.UpdateCurrentTextBox(currentTextBox);
            canvasGroup.alpha = 1f;
            OnNotificationMessage?.Invoke(message);
            
            delayedNotificationRoutine = null;
        }

        void RequestHideNotification()
        {
            Invoke(nameof(HideNotification), hideDelay);
        }

        void HideNotification()
        {
            canvasGroup.alpha = 0f;
        }
    }
}

