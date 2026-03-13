using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class Silhouette : MonoBehaviour
	{
		Transform silhouettePos;
		Camera cameraPos;
		[SerializeField] LayerMask playerLayer;
		[SerializeField] float playerWatchingFOV = 0.95f;
		[SerializeField] float playerCanSeeFOV = 0.5f;


		bool playerWatching;

        void Start()
        {
            silhouettePos = transform;
			cameraPos = Camera.main;
			UpdateSilhouetteVisibility(false);
        }

        void Update()
        {
			playerWatching = PlayerVision.IsPointVisible(cameraPos, silhouettePos.position, playerWatchingFOV, playerLayer);
			if (playerWatching)
			{
				//Debug.Log("Player is looking at silhouette");
				// Send this to InsanityManager
				// ScreenFader + disappear after being watched too long
			}
			else
			{
				//Debug.Log("Cannot see silhouette");
				// if player WAS looking, but no longer looking, despawn. Also add a small delay
				// Use screenfader here too
			}
        }

		public bool CanPlayerSeeSilhouetteSpawn()
		{
			bool playerCanSee = PlayerVision.IsPointVisible(cameraPos, silhouettePos.position, playerCanSeeFOV, playerLayer);
			if (playerCanSee)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void UpdateSilhouetteVisibility(bool visible)
		{
			Collider collider = GetComponentInChildren<BoxCollider>();
			SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();

			if (renderer == null || collider == null) return;

			renderer.enabled = visible;
			collider.enabled = visible;
		}

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
