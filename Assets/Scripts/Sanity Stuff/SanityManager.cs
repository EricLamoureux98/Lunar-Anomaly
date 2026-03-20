using System;
using LunarAnomaly.Player;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
		// Sanity should drain when losing oxygen and witnessing events
	public class SanityManager : MonoBehaviour
	{		
		[Header("Sanity Behaviour")]
		[SerializeField] float maxSanity = 20f; // Should be around 15 - 20 minutes of gameplay
		[SerializeField] float trySpawnSilhouetteTime = 90f;		
		[SerializeField] float spikeAmount = 1f; // Consider allowing events to decide
		[SerializeField] float extraDrainAmount = 0.15f; // Consider allowing events to decide

		[Header("Private trackers")]
		public float currentSanity; // Public for testing
		float silhouetteSpawnChance = 0f;
		public bool sanityDraining; // public for testing
		float bonusChance;
		float spawnTimer;

		public SanityState sanityState; // Public for testing

		// To SilhouetteManager
		public static event Action OnSilhouetteRequest;		
		// To PlayerState
		public static event Action OnInsanity;

		 void OnEnable()
        {
            AtmosphereTracker.OnPressurized += HandleSanityStateChange;
			Silhouette.OnSilhouetteWatched += InsanityExtraDrain;
			Silhouette.OnSilhouetteVanished += InsanitySpike;
        }

        void OnDisable()
        {
            AtmosphereTracker.OnPressurized -= HandleSanityStateChange;
			Silhouette.OnSilhouetteWatched -= InsanityExtraDrain;
			Silhouette.OnSilhouetteVanished -= InsanitySpike;
        }

        void Start()
        {
            currentSanity = maxSanity;
			ChangeState(SanityState.HighSanity);
        }

        void Update()
        {
			SanityDrain();
			HandleSilhouetteSpawning();
        }

        void SanityDrain()
		{
			if (sanityDraining && currentSanity >= 0)
			{
            	currentSanity -= Time.deltaTime / 60f;	

				// Remove these magic numebrs
				if (currentSanity <= maxSanity * 0.1f)
				{
					ChangeState(SanityState.CriticalSanity);
				}
				else if (currentSanity <= maxSanity * 0.3f)
				{
					ChangeState(SanityState.LowSanity);
				}
				else if (currentSanity <= maxSanity * 0.6f)
				{
					ChangeState(SanityState.MediumSanity);
				}
				else
				{
					ChangeState(SanityState.HighSanity);
				}			
			}

			if (currentSanity <= 0)
			{
				OnInsanity?.Invoke();
			}
		}

		void HandleSanityStateChange(bool sane)
		{
			sanityDraining = !sane;
		}

		void InsanitySpike()
		{
			currentSanity -= spikeAmount;
		}

		void InsanityExtraDrain()
		{
			currentSanity -= extraDrainAmount * Time.deltaTime;
		}

		void HandleSilhouetteSpawning()
		{
			if (sanityState == SanityState.HighSanity) return;

			float currentChance = silhouetteSpawnChance + bonusChance;

			spawnTimer += Time.deltaTime;

			if (spawnTimer >= trySpawnSilhouetteTime)
			{
				if (UnityEngine.Random.value < currentChance)
				{
					OnSilhouetteRequest?.Invoke();
					spawnTimer = 0f;
					bonusChance = 0f;
				}
				else
				{
					bonusChance += 0.05f;
					spawnTimer = 0f;
				}
			}
		}

		void ChangeState(SanityState newState)
		{
			if (newState == sanityState) return;

			ExitState(sanityState);
			sanityState = newState;
			EnterState(newState);
		}

		void EnterState(SanityState state)
		{
			switch (state)
			{
				case SanityState.HighSanity:
					silhouetteSpawnChance = 0f; // <-- Maybe remove these magic numbers
					break;	

				case SanityState.MediumSanity:
					silhouetteSpawnChance = 0.25f;
					break;

				case SanityState.LowSanity:
					silhouetteSpawnChance = 0.5f;
					break;

				case SanityState.CriticalSanity:
					silhouetteSpawnChance = 1f;
					break;

			}
		}

		void ExitState(SanityState state)
		{
			// Immediately spawn a silhouette when changing state
			switch (state)
			{
				case SanityState.HighSanity:	
					OnSilhouetteRequest?.Invoke();				
					break;	

				case SanityState.MediumSanity:
					OnSilhouetteRequest?.Invoke();
					break;

				case SanityState.LowSanity:
					OnSilhouetteRequest?.Invoke();
					break;

				case SanityState.CriticalSanity:
					break;

			}
		}
    }
}

public enum SanityState
{
	HighSanity, 
	MediumSanity, 
	LowSanity, 
	CriticalSanity 
}
