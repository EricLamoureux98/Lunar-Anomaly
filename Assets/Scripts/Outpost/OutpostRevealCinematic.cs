using System;
using System.Collections;
using LunarAnomaly.Player;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class OutpostRevealCinematic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ScreenFader screenFader;
        [SerializeField] LayerMask playerLayer;
        SpriteRenderer silhouette;

        [Header("Event Settings")]
        [SerializeField] float playerWatchingFOV = 0.6f;
        [SerializeField] float waitBeforeFlash = 2.5f;
        [SerializeField] float fadeBlackTime = 0.75f;
        [SerializeField] Transform playerTeleportPos;

        bool silhouetteActive;
        bool playerWatching;
        Camera cameraPos;

        // To PlayerState
        public static event Action<Transform, TeleportType> OnOutpostCinematicTeleport;
        // To OutpostController
        public static event Action OnDisableOutpost;
        // To SanityManager
        public static event Action OnActivateSanitySystem;

        void OnEnable()
        {
            OutpostController.OnCinematicSilhouetteSpawn += ShowSilhouette;
        }

        void OnDisable()
        {
            OutpostController.OnCinematicSilhouetteSpawn -= ShowSilhouette;
        }

        void Start()
        {
            silhouette = GetComponentInChildren<SpriteRenderer>();
            cameraPos = Camera.main;
        }

        void Update()
        {
            if (silhouetteActive)
                CheckSilhouetteSeen();
        }

        void ShowSilhouette()
        {
            silhouetteActive = true;
            silhouette.enabled = true;
        }

        void CheckSilhouetteSeen()
        {
            playerWatching = PlayerVision.IsPointVisible(cameraPos, transform, playerWatchingFOV, playerLayer);

            if (playerWatching)
            {
                StartCoroutine(nameof(SilhouetteEvent));
            }
        }

        IEnumerator SilhouetteEvent()
        {
            silhouetteActive = false;

            SoundManager.PlaySound(SoundType.AlienSeenFirstTime, 2f, false);

            yield return new WaitForSeconds(waitBeforeFlash);

            Silhouette.OnSilhouetteFlash?.Invoke(fadeBlackTime);
            OnOutpostCinematicTeleport?.Invoke(playerTeleportPos, TeleportType.Cinematic);
            OnDisableOutpost?.Invoke();
            silhouette.enabled = false;
            OnActivateSanitySystem?.Invoke();

            yield return new WaitForSeconds(fadeBlackTime);
            Destroy(gameObject);
        }
    }
}

