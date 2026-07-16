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

        readonly Queue<NotificationMessage> notificationQueue = new Queue<NotificationMessage>();        Coroutine queueRoutine;
        bool waitingForReveal;

        // To UpdateText
        public static event Action<NotificationMessage> OnNotificationMessage;

        void OnEnable()
        {
            TerminalUI.OnRequestNotification += RequestNotification;
            TerminalUI.OnRequestNotificationDelayed += RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed += OnTypewriterComplete;
            //Typewriter.OnCompleteTextRevealed += RequestHideNotification;
        }

        void OnDisable()
        {
            TerminalUI.OnRequestNotification -= RequestNotification;
            TerminalUI.OnRequestNotificationDelayed -= RequestNotificationDelayed;
            Typewriter.OnCompleteTextRevealed -= OnTypewriterComplete;
            //Typewriter.OnCompleteTextRevealed -= RequestHideNotification;
        }

        void RequestNotification(NotificationMessage message)
        {
            Enqueue(message);

            // updateText.UpdateCurrentTextBox(currentTextBox);
            // canvasGroup.alpha = 1f;
            // OnNotificationMessage?.Invoke(message);
        }

        void RequestNotificationDelayed(NotificationMessage message, float delay)
        {
            StartCoroutine(DelayedEnqueue(message, delay));

            // if (delayedNotificationRoutine != null) return;
            // delayedNotificationRoutine = StartCoroutine(DelayedNotification(message, delay));
        }

        IEnumerator DelayedEnqueue(NotificationMessage message, float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return new WaitUntil(() => !terminalUI.TerminalActive);
            Enqueue(message);

            //CancelInvoke(nameof(HideNotification));

            // updateText.UpdateCurrentTextBox(currentTextBox);
            // canvasGroup.alpha = 1f;
            // OnNotificationMessage?.Invoke(message);
            
            // delayedNotificationRoutine = null;
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
        }

        // void RequestHideNotification()
        // {
        //     Invoke(nameof(HideNotification), hideDelay);
        // }

        // void HideNotification()
        // {
        //     canvasGroup.alpha = 0f;
        // }
    }
}

