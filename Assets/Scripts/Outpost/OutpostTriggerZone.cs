using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using UnityEngine;
	
namespace LunarAnomaly.UI
{
	public class OutpostTriggerZone : MonoBehaviour
	{
		[SerializeField] OutpostPrompt prompt;
		[SerializeField] CanvasGroup canvasGroup;

		bool playerInside;
        [SerializeField] bool triggerActive;

		// To OutpostController
		public static event Action<OutpostPrompt> OnInteract;

        void OnEnable()
        {
            InputHandler.OnInteractPressed += HandleInteract;
            OutpostController.OnTriggerZoneActive += HandleActive;
        }

        void OnDisable()
        {
            InputHandler.OnInteractPressed -= HandleInteract;
            OutpostController.OnTriggerZoneActive -= HandleActive;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
			{
				playerInside = true;
				if (canvasGroup != null) canvasGroup.alpha = 1f;

				//OutpostController.OnOutpostUIUpdate?.Invoke(prompt, true);
			}
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
			{
				playerInside = false;
				if (canvasGroup != null) canvasGroup.alpha = 0f;

				//OutpostController.OnOutpostUIUpdate?.Invoke(prompt, false);
			}
        }

        void HandleActive(OutpostPrompt updatedPrompt, bool active)
        {
            if (updatedPrompt == prompt)
                triggerActive = active;
        }

        void HandleInteract()
        {
            if (playerInside && triggerActive)
			{
				OnInteract?.Invoke(prompt);
                //Debug.Log($"{prompt} interacted with");
			}
        }
    }
}
