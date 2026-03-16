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

		[SerializeField] float maxWatchTime = 4f;
		[SerializeField] float fadeBlackTime = 0.75f;
		float watchTime;

		bool playerWatching;
		bool playerWasWatching;
		bool silhouetteEnabled;

		// To UIManager
		public static event Action<float> OnSilhouetteFlash;
		public static event Action PlayerWatchingSilhouette;

		// To SanityManager
		public static event Action OnSilhouetteWatched;

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

        void Update()
        {
			if (silhouetteEnabled)
				CheckPlayerWatching();
        }

		void CheckPlayerWatching()
		{
			playerWatching = PlayerVision.IsPointVisible(cameraPos, silhouettePos, playerWatchingFOV, playerLayer);

			if (playerWatching)
			{
				// Send this to SanityManager
				OnSilhouetteWatched?.Invoke();

				playerWasWatching = true;

				if (watchTime < maxWatchTime)
				{
					watchTime += Time.deltaTime;
				}	
				else
				{
					OnSilhouetteFlash?.Invoke(fadeBlackTime);
					UpdateSilhouetteVisibility(false);
				}			
			}
			
			if (!SilhouetteOnScreen() && playerWasWatching)
			{
				// Consider adding a delay and min watch timer here!
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

			// Double check this
			playerWasWatching = false;
			watchTime = 0f;
		}

		////////////////// OLD - Has been replaced by PlayerVision /////////////////////

        // void CheckIfPlayerWatching()
		// {
		// 	Vector3 directionToSilhouette = (silhouettePos.position - cameraPos.transform.position).normalized;
		// 	float dot = Vector3.Dot(cameraPos.transform.forward, directionToSilhouette);

		// 	if (dot > 0.95f)
		// 	{
		// 		RaycastHit hit;
		// 		float distanceToSilhouette = Vector3.Distance(cameraPos.transform.position, silhouettePos.position);
		// 		Debug.DrawRay(cameraPos.transform.position, directionToSilhouette * distanceToSilhouette, Color.red);

		// 		int mask = ~playerLayer;

		// 		if (Physics.Raycast(cameraPos.transform.position, directionToSilhouette, out hit, distanceToSilhouette, mask))
		// 		{
		// 			if (hit.collider.CompareTag("Silhouette"))
		// 			{
		// 				Debug.Log("Player is looking at silhouette"); 				
		// 			}
		// 		}
		// 	}
		// 	else if (dot <= 0)
		// 	{
		// 		Debug.Log("Player cannot see silhouette");
		// 	}
		// }
    }
}
