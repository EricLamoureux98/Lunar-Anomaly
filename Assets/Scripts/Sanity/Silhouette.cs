using System;
using System.Collections;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class Silhouette : MonoBehaviour
	{
		[Header("References")]
		SpriteRenderer spriteRenderer;

		[Header("Detection")]
		[SerializeField] LayerMask playerLayer;
		[SerializeField] float playerWatchingFOV = 0.95f;
		[SerializeField] float playerCanSeeFOV = 0.5f;
		[SerializeField] Transform visibilityHitbox;
		Transform silhouetteTransform;
		Vector3 silhouetteStartingPos;
		GameObject playerObject;
		Camera cameraPos;

		[Header("Behaviour")]
		[SerializeField] float maxWatchTime = 4f;
		[SerializeField] float minWatchBeforeVanish = 1.5f;
		// [SerializeField] float fadeBlackTime = 0.75f;
		[SerializeField] float playerTooCloseDistance = 150f;
		public float watchTime; // public for testing

		public bool playerWatching; // public for testing
		public bool silhouetteOnScreen; // public for testing
		public bool playerWasWatching; // public for testing
		public bool silhouetteEnabled; // public for testing
		bool debugNotif;

		// To UIManager - Called in OutpostRevealCinematic
		public static Action OnSilhouetteFlash;
		// To SanityManager
		public static event Action OnSilhouetteWatched;
		public static event Action OnSilhouetteVanished;

        void Awake()
        {
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start()
        {
            silhouetteTransform = transform;
			silhouetteStartingPos = transform.position;
			cameraPos = Camera.main;
			UpdateSilhouetteVisibility(false);

			playerObject = GameObject.FindWithTag("Player");
        }

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
				MaintainDistanceFromPlayer();
			}
        }

		void LookAtPlayer()
		{
			if (playerObject == null) return;

			silhouetteTransform.LookAt(playerObject.transform);
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
					StartCoroutine(MoveSilhouette());
					OnSilhouetteVanished?.Invoke();
					silhouetteEnabled = false;
					//OnSilhouetteFlash?.Invoke();
					//SoundManager.PlaySound(SoundType.Ambience);
				}			
			}
			else
			{
				// Reset to 0 if not watched vv breaks below logic
				//watchTime = 0f;
			}
			
			if (!SilhouetteOnScreen() && playerWasWatching && watchTime > minWatchBeforeVanish)
			{
				UpdateSilhouetteVisibility(false);				
			}
		}

		void MaintainDistanceFromPlayer()
		{
			float distance = Vector3.Distance(cameraPos.transform.position, silhouetteTransform.position);

			if (distance <= playerTooCloseDistance)
				UpdateSilhouetteVisibility(false);
		}

		public bool PlayerCanSeeSilhouette(Transform player, LayerMask obstacleMask)
		{
			Vector3 origin = visibilityHitbox.position;
			Vector3 targetPos = player.position; // + Vector3.up * 1.15f;

			Vector3 dir = (targetPos - origin).normalized;
			float distance = Vector3.Distance(origin, targetPos);

			int mask = obstacleMask;

			if (Physics.Raycast(origin, dir, out RaycastHit hit, distance, mask))
			{
				return hit.transform == player || hit.transform.IsChildOf(player);
			}

			return false;
		}

		public bool SilhouetteOnScreen()
		{
			return PlayerVision.IsPointVisible(cameraPos, visibilityHitbox, playerCanSeeFOV, playerLayer);
		}

		public float SilhouetteDistance()
		{
			return Vector3.Distance(cameraPos.transform.position, silhouetteTransform.position);
		}

		public void UpdateSilhouetteVisibility(bool visible)
		{
			if (spriteRenderer == null) return;

			if (visible) transform.position = silhouetteStartingPos;

			silhouetteEnabled = visible;
			spriteRenderer.enabled = visible;

			playerWasWatching = false;
			debugNotif = false;
			watchTime = 0f;
		}

		IEnumerator MoveSilhouette()
		{
			Vector3 targetPos = silhouetteStartingPos - new Vector3(0, 40f, 0);
			float duration = 3f;
			float elapsed = 0f;

			while (elapsed < duration)
			{
				elapsed += Time.deltaTime;

				float t = elapsed / duration;
				transform.position = Vector3.Lerp(silhouetteStartingPos, targetPos, t);

				yield return null;
			}

			transform.position = targetPos;
			UpdateSilhouetteVisibility(false);
		}
    }
}
