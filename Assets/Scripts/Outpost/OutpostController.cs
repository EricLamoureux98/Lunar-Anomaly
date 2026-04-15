using System;
using System.Collections;
using LunarAnomaly.UI;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class OutpostController : MonoBehaviour
	{
		AtmosphereZone atmosphereZone;

		bool outpostActive;
		bool isPowered;
		bool outpostRepaired;

		[Header("Airlock")]
		[SerializeField] float pressurizationTime = 3f;
		bool playerInside = false;
		bool isCycling = false;

		// To OutpostUI
		public static event Action<OutpostPrompt, bool> OnOutpostUIUpdate;
		public static event Action OnOutpostHideAllUI;

        void OnEnable()
        {
            OutpostRepair.OnOutpostRepaired += HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract += HandleInteract;
        }

        void OnDisable()
        {
            OutpostRepair.OnOutpostRepaired -= HandleOutpostRepaired;
			OutpostTriggerZone.OnInteract -= HandleInteract;
        }

        void Awake()
        {
			atmosphereZone = GetComponentInChildren<AtmosphereZone>();
        }

		// ***** FINISH THIS ******
		// Enum probably needs a non for the repair nodes
		void HandleInteract(OutpostPrompt prompt)
		{
			switch (prompt)
			{
				case OutpostPrompt.PowerPanel:
					TryOpenPowerbox();
					break;
			}
		}

		void HandleOutpostRepaired()
		{
			outpostRepaired = true;
		}

		void TryOpenPowerbox()
		{
			
		}

		// Exterior powerbox
        void TryEnablePower()
		{
			if (outpostRepaired && !isPowered)
			{
				isPowered = true;
				
				// Allow player to enter
				// Cycle airlock
			}
			// Maybe notify player if not fully repaired
		}

		// Interior button
		void TryActivateOutpost()
		{
			if (isPowered && !outpostActive)
			{
				outpostActive = true;
				// Handle insanity start
				// Quest complete
			}
		}

		public void TryEnterOutpost()
		{
			if (isPowered && outpostActive)
			{
				// Open door
			}
		}

        void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			playerInside = true;
			Debug.Log("Player entered outpost");
        }

        void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

			playerInside = false;
			Debug.Log("Player exited outpost");
        }

        void WaitForPlayerEnter()
		{
			if (playerInside)
			{
				if (isCycling) return;

				StartCoroutine(CycleAtmosphere());
			}
		}

		IEnumerator CycleAtmosphere()
		{
			yield return new WaitForSeconds(1);
		}
    }
}


// Give the player a reason to repair the outpost. "Something caused it to break down" 

// Player repairs Outpost exterior
// Power box can be used
// Player enables power
// Door becomes active and can be opened
// Player enters
// Outpost pressurizes and door closes
// Player can activate Outpost
// Player presses activate button
// Power flickers and sounds of powering up
// "Log downloaded" - voice clip starts to play
// Player goes to exit and open door
// Door opens and a silhouette is seen for a short moment
// Cut to black and play insanity sounds. Silhouette is gone
// Insanity system starts 0 - 100%
// Return to base