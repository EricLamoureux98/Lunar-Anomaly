using System;
using LunarAnomaly.Input;
using UnityEngine;
	
namespace LunarAnomaly.UI
{
	public class OutpostTriggerZone : MonoBehaviour
	{
		[SerializeField] OutpostPrompt prompt;
		[SerializeField] CanvasGroup canvasGroup;

		bool playerInside;

		// To OutpostController
		public static event Action<OutpostPrompt> OnInteract;

        void OnEnable()
        {
            InputHandler.OnInteractPressed += HandleInteract;
        }

        void OnDisable()
        {
            InputHandler.OnInteractPressed -= HandleInteract;
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

        void HandleInteract()
        {
            if (playerInside)
			{
				OnInteract?.Invoke(prompt);
			}
        }
    }
}
