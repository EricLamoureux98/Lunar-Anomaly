using System;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class Silhouette : MonoBehaviour
	{
		[Header("References")]
		Collider silhouetteCollider;
		SpriteRenderer spriteRenderer;

		[Header("Detection")]
		[SerializeField] LayerMask playerLayer;
		[SerializeField] float playerWatchingFOV = 0.95f;
		[SerializeField] float playerCanSeeFOV = 0.5f;
		Transform silhouettePos;
		Camera cameraPos;

		[Header("Behaviour")]
		[SerializeField] float maxWatchTime = 4f;
		[SerializeField] float minWatchBeforeVanish = 1.5f;
		[SerializeField] float fadeBlackTime = 0.75f;
		float watchTime;

		bool playerWatching;
		bool playerWasWatching;
		bool silhouetteEnabled;
		bool debugNotif;

		// To UIManager - Called in OutpostRevealCinematic
		public static Action<float> OnSilhouetteFlash;

		// To SanityManager
		public static event Action OnSilhouetteWatched;
		public static event Action OnSilhouetteVanished;

        void Awake()
        {
            silhouetteCollider = GetComponentInChildren<BoxCollider>();
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start()
        {
            silhouettePos = transform;
			cameraPos = Camera.main;
			UpdateSilhouetteVisibility(false);
        }

		// Consider adding a max active time
        void Update()
        {
			if (silhouetteEnabled)
			{
				if (!debugNotif)
				{
					Debug.Log("Silhouette Spawned: " + gameObject.name);
					debugNotif = true;
				}

				CheckPlayerWatching();
			}
        }

		void CheckPlayerWatching()
		{
			playerWatching = PlayerVision.IsPointVisible(cameraPos, silhouettePos, playerWatchingFOV, playerLayer);

			if (playerWatching)
			{
				OnSilhouetteWatched?.Invoke();

				playerWasWatching = true;

				if (watchTime < maxWatchTime)
				{
					watchTime += Time.deltaTime;
				}	
				else
				{
					OnSilhouetteFlash?.Invoke(fadeBlackTime);
					OnSilhouetteVanished?.Invoke();
					UpdateSilhouetteVisibility(false);
					SoundManager.PlaySound(SoundType.Ambience);
				}			
			}
			else
			{
				watchTime = 0f;
			}
			
			if (!SilhouetteOnScreen() && playerWasWatching && watchTime > minWatchBeforeVanish)
			{
				// Consider adding a delay
				UpdateSilhouetteVisibility(false);
				
			}
		}

		public bool SilhouetteOnScreen()
		{
			return PlayerVision.IsPointVisible(cameraPos, silhouettePos, playerCanSeeFOV, playerLayer);
		}

		public float SilhouetteDistance()
		{
			return Vector3.Distance(cameraPos.transform.position, silhouettePos.transform.position);
		}

		public void UpdateSilhouetteVisibility(bool visible)
		{
			if (spriteRenderer == null || silhouetteCollider == null) return;

			silhouetteEnabled = visible;
			spriteRenderer.enabled = visible;
			silhouetteCollider.enabled = visible;

			playerWasWatching = false;
			debugNotif = false;
			watchTime = 0f;
		}
    }
}
