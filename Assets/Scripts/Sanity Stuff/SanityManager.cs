using System;
using LunarAnomaly.Player;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class SanityManager : MonoBehaviour
	{		
		[SerializeField] float maxSanity = 20f; // Should be around 15 - 20 minutes

		// Consider allowing events to decide
		[SerializeField] float spikeAmount = 1f;
		[SerializeField] float extraDrainAmount = 0.15f;
		public float currentSanity; // Public for testing
		//float silhouetteSpawnChance; // Update with ENUM

		// To SilhouetteManager
		public static event Action OnSilhouetteRequest;
		
		// Sanity should drain when losing oxygen and witnessing events
		public bool sanityDraining; // public for testing

		 void OnEnable()
        {
            AtmosphereTracker.OnPressurized += HandleSanityStateChange;
			Silhouette.OnSilhouetteWatched += InsanityExtraDrain;
        }

        void OnDisable()
        {
            AtmosphereTracker.OnPressurized -= HandleSanityStateChange;
			Silhouette.OnSilhouetteWatched -= InsanityExtraDrain;
        }

        void Start()
        {
            currentSanity = maxSanity;
        }

        void Update()
        {
			SanityDrain();
        }

        void SanityDrain()
		{
			if (sanityDraining && currentSanity >= 0)
			{
            	currentSanity -= Time.deltaTime / 60f;	

				if(currentSanity >= maxSanity * 0.25)
				{
					//OnSilhouetteRequest?.Invoke();
				} 			
			}
		}

		void HandleSanityStateChange(bool sane)
		{
			sanityDraining = !sane;
		}

		// Implement spike when player watches until vanish
		void InsanitySpike()
		{
			currentSanity -= spikeAmount;
		}

		void InsanityExtraDrain()
		{
			currentSanity -= extraDrainAmount * Time.deltaTime;
		}

		// void RequestSilhouette()
		// {
		// 	if(currentInsanity >= maxInsanity * 0.25)
		// 	{
		// 		OnSilhouetteRequest?.Invoke();
		// 	} 	
		// }
    }
}

// Implement sanity states
public enum InsanityState
{
	HighSanity, // 75
	MediumSanity, // 50
	LowSanity, // 25
	CriticalSanity // 10
}
