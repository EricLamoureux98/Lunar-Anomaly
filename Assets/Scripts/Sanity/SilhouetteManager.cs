using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class SilhouetteManager : MonoBehaviour
	{
        [SerializeField] float minSilhouetteDistance = 500f;

		Silhouette[] silhouetteSpawnPoints;
        Silhouette activeSilhouette;

        void OnEnable()
        {
            SanityManager.OnSilhouetteRequest += RequestSilhouette;
        }

        void OnDisable()
        {
            SanityManager.OnSilhouetteRequest -= RequestSilhouette;
        }

        void Awake()
        {
            silhouetteSpawnPoints = FindObjectsByType<Silhouette>(FindObjectsSortMode.None);
        }

        void Start()
        {
            // For testing
            //SelectSilhouette();
        }

        // This should be called by insanity manager
        void RequestSilhouette()
        {
            SelectSilhouette();
        }

        void SelectSilhouette()
        {
            // Consider shuffle then pick in order so that there are no repeats!
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

                if (!candidate.SilhouetteOnScreen() && candidate.SilhouetteDistance() > minSilhouetteDistance)
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
