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
		[SerializeField] Transform visibilityHitbox;
		[SerializeField] Transform silhouettePos;
		GameObject playerPos;
		Camera cameraPos;

		[Header("Behaviour")]
		[SerializeField] float maxWatchTime = 4f;
		[SerializeField] float minWatchBeforeVanish = 1.5f;
		[SerializeField] float fadeBlackTime = 0.75f;
		float watchTime;

		public bool playerWatching; // public for testing
		public bool playerWasWatching; // public for testing
		bool silhouetteEnabled; // public for testing
		bool debugNotif;

		// To UIManager - Called in OutpostRevealCinematic
		public static Action OnSilhouetteFlash;
		// To SanityManager
		public static event Action OnSilhouetteWatched;
		public static event Action OnSilhouetteVanished;

        void Awake()
        {
            silhouetteCollider = GetComponentInChildren<SphereCollider>();
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start()
        {
            silhouettePos = transform;
			cameraPos = Camera.main;
			UpdateSilhouetteVisibility(false); 

			playerPos = GameObject.FindWithTag("Player");
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
				LookAtPlayer();
			}
        }

		void LookAtPlayer()
		{
			if (playerPos == null) return;

			silhouettePos.LookAt(playerPos.transform);
		}

		void CheckPlayerWatching()
		{
			playerWatching = PlayerVision.IsPointVisible(cameraPos, visibilityHitbox, playerWatchingFOV, playerLayer);

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
					OnSilhouetteFlash?.Invoke();
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
			return Vector3.Distance(cameraPos.transform.position, silhouettePos.position);
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
