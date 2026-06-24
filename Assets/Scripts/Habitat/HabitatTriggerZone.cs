using System;
using LunarAnomaly.Input;
using UnityEngine;

namespace LunarAnomaly.UI
{
    public class HabitatTriggerZone : MonoBehaviour
    {
        [SerializeField] HabitatPrompt prompt;
        [SerializeField] CanvasGroup canvasGroup;

        bool playerInside;
        [SerializeField] bool triggerActive;

        // To HabitatController
        public static event Action<HabitatPrompt> OnInteract;

        void OnEnable()
        {
            InputHandler.OnInteractPressed += HandleInteract;
            HabitatController.OnTriggerZoneActive += HandleActive;
        }

        void OnDisable()
        {
            InputHandler.OnInteractPressed -= HandleInteract;
            HabitatController.OnTriggerZoneActive -= HandleActive;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
			{
				playerInside = true;
				if (canvasGroup != null) canvasGroup.alpha = 1f;
			}
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
			{
				playerInside = false;
				if (canvasGroup != null) canvasGroup.alpha = 0f;
			}
        }

        void HandleActive(HabitatPrompt updatedPrompt, bool active)
        {
            if (updatedPrompt == prompt)
                triggerActive = active;
        }

        void HandleInteract()
        {
            if (playerInside && triggerActive)
			{
				OnInteract?.Invoke(prompt);
                Debug.Log($"{prompt} interacted with");
			}
        }
    }
}