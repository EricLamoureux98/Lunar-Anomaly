using System;
using System.Collections;
using System.Collections.Generic;
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

        readonly Queue<NotificationMessage> notificationQueue = new Queue<NotificationMessage>();        
        Coroutine queueRoutine;

        bool waitingForReveal;
        public bool WaitingForReveal => waitingForReveal;

        // To UpdateText
        public static event Action<NotificationMessage> OnNotificationMessage;
        // To TerminalUI
        public event Action OnWaitingForRevealChanged;

        void OnEnable()
        {
            TerminalUI.OnRequestNotification += RequestNotification;
            TerminalUI.OnRequestNotificationDelayed += RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed += OnTypewriterComplete;
        }

        void OnDisable()
        {
            TerminalUI.OnRequestNotification -= RequestNotification;
            TerminalUI.OnRequestNotificationDelayed -= RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed -= OnTypewriterComplete;
        }

        void RequestNotification(NotificationMessage message)
        {
            Enqueue(message);
        }

        void RequestNotificationDelayed(NotificationMessage message, float delay)
        {
            StartCoroutine(DelayedEnqueue(message, delay));
        }

        IEnumerator DelayedEnqueue(NotificationMessage message, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => !terminalUI.TerminalActive);
            Enqueue(message);
        }

        void Enqueue(NotificationMessage message)
        {
            notificationQueue.Enqueue(message);
            queueRoutine ??= StartCoroutine(ProcessQueue());
        }

        IEnumerator ProcessQueue()
        {
            while (notificationQueue.Count > 0)
            {
                var message = notificationQueue.Dequeue();

                waitingForReveal = true;
                OnWaitingForRevealChanged?.Invoke();
                updateText.UpdateCurrentTextBox(currentTextBox);
                canvasGroup.alpha = 1f;
                OnNotificationMessage?.Invoke(message);

                yield return new WaitUntil(() => !waitingForReveal);
                yield return new WaitForSeconds(hideDelay);

                if (notificationQueue.Count == 0)
                    canvasGroup.alpha = 0f;
            }

            queueRoutine = null;
        }

        void OnTypewriterComplete()
        {
            waitingForReveal = false;
            OnWaitingForRevealChanged?.Invoke();
        }
    }
}

