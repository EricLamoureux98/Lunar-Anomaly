using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
	
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
		// Transform silhouetteTransform;
		Vector3 silhouetteStartingPos;
		GameObject playerObject;
		Camera cameraPos;

		[Header("Behaviour")]
		[SerializeField] float maxWatchTime = 4f;
		[SerializeField] float minWatchBeforeVanish = 1.5f;
		[SerializeField] float playerTooCloseDistance = 150f;
		float maxTimeBeforeRespawn;
		public float watchTime; // public for testing

		public bool playerWatching; // public for testing
		public bool silhouetteOnScreen; // public for testing
		public bool playerWasWatching; // public for testing
		public bool silhouetteEnabled; // public for testing
		bool debugNotif;

		Coroutine moveSilhouetteRoutine;
		Coroutine respawnRoutine;
		Coroutine dissolveRoutine;

		// To ScreenEffect
		public static event Action OnSilhouetteFlash; // not yet used
		// To SanityManager
		public static event Action OnSilhouetteWatched;
		public static event Action OnSilhouetteVanished;

        void Awake()
        {
			spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        void Start()
        {
            // silhouetteTransform = transform;
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

			transform.LookAt(playerObject.transform);
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
					RandomSilhouetteDisappearance();
						
					OnSilhouetteVanished?.Invoke();
					silhouetteEnabled = false;
				}			
			}
			
			if (!SilhouetteOnScreen() && playerWasWatching && watchTime > minWatchBeforeVanish)
			{
				UpdateSilhouetteVisibility(false);				
			}
		}

		void MaintainDistanceFromPlayer()
		{
			float distance = Vector3.Distance(cameraPos.transform.position, transform.position);

			if (distance <= playerTooCloseDistance)
				UpdateSilhouetteVisibility(false);
		}

		public bool PlayerCanSeeSilhouette(Transform player, LayerMask obstacleMask)
		{
			Vector3 origin = visibilityHitbox.position;

			Vector3 dir = (player.position - origin).normalized;
			float distance = Vector3.Distance(origin, player.position);

			return !Physics.Raycast(origin, dir, distance, obstacleMask);
		}

		public bool SilhouetteOnScreen()
		{
			return PlayerVision.IsPointVisible(cameraPos, visibilityHitbox, playerCanSeeFOV, playerLayer);
		}

		public float SilhouetteDistance()
		{
			return Vector3.Distance(cameraPos.transform.position, transform.position);
		}

		public void UpdateSilhouetteVisibility(bool visible)
		{
			if (spriteRenderer == null) return;

			if (visible) transform.position = silhouetteStartingPos;

			silhouetteEnabled = visible;
			spriteRenderer.enabled = visible;

			Color alpha = spriteRenderer.color;
			alpha.a = 0.8f;
			spriteRenderer.color = alpha;

			playerWasWatching = false;
			debugNotif = false;
			watchTime = 0f;

			if (respawnRoutine == null && visible)
				respawnRoutine = StartCoroutine(CheckRespawnRoutine());
		}

		void RandomSilhouetteDisappearance()
		{
			float chance = Random.value;

			if (chance < 0.6f)
			{
				Debug.Log("Silhouette Sinking");
				if (moveSilhouetteRoutine == null)
					moveSilhouetteRoutine = StartCoroutine(MoveSilhouetteRoutine());
			}
			else if (chance < 0.8f)
			{
				Debug.Log("Silhouette Dissolving");
				if (dissolveRoutine == null)
					dissolveRoutine = StartCoroutine(DissolveSilhouetteRoutine());
			}
			else
			{
				Debug.Log("Silhouette Blinking");
				OnSilhouetteFlash?.Invoke();
				UpdateSilhouetteVisibility(false);
			}
		}

		IEnumerator MoveSilhouetteRoutine()
		{
			Vector3 targetPos = silhouetteStartingPos - new Vector3(0, 40f, 0);
			float duration = 1f;
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
			moveSilhouetteRoutine = null;
			yield break;
		}

		IEnumerator DissolveSilhouetteRoutine()
		{
			Color visible = spriteRenderer.color;
			Color notVisible = visible;
			notVisible.a = 0f;

			float duration = 1f;
			float timer = 0f;
			
			while (timer < duration)
			{
				timer += Time.deltaTime;
				spriteRenderer.color = Color.Lerp(visible, notVisible, timer);
				yield return null;				
			}

			UpdateSilhouetteVisibility(false);
			dissolveRoutine = null;
			yield break;
		}

		IEnumerator CheckRespawnRoutine()
		{
			maxTimeBeforeRespawn = Random.Range(30f, 50f);
			float activeTime = 0f;

			while (activeTime < maxTimeBeforeRespawn)
			{
				if (watchTime > 0) yield break;

				activeTime += Time.deltaTime;
				yield return null;
			}

			UpdateSilhouetteVisibility(false);
			SanityManager.OnSilhouetteRequest?.Invoke();
			
			respawnRoutine = null;
			yield break;
		}
    }
}
