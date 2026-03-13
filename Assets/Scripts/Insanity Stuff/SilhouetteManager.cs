using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class SilhouetteManager : MonoBehaviour
	{
		[SerializeField] GameObject silhouettePrefab;
		Silhouette[] silhouetteSpawnPoints;
        Silhouette activeSilhouette;

        void Awake()
        {
            silhouetteSpawnPoints = FindObjectsByType<Silhouette>(FindObjectsSortMode.None);
        }

        void Start()
        {
            // For testing
            SelectSilhouette();
        }

        // This should be called by insanity manager
        void RequestSilhouette()
        {
            
        }

        void SelectSilhouette()
        {
            if (silhouetteSpawnPoints.Length == 0) return;

            int attempts = 0;
            int maxAttemtps = silhouetteSpawnPoints.Length;

            while (attempts < maxAttemtps)
            {
                int index = Random.Range(0, silhouetteSpawnPoints.Length);
                Silhouette candidate = silhouetteSpawnPoints[index];

                if (candidate == null)
                {
                    attempts++;
                    continue;
                }

                // Add a distance check so Silhouettes spawn far away
                if (!candidate.SilhouetteOnScreen())
                {
                    activeSilhouette = candidate;
                    activeSilhouette.UpdateSilhouetteVisibility(true);
                    return;
                }

                attempts++;
            }

            Debug.Log("No valid silhouette spawn points found");
        }
    }
}
