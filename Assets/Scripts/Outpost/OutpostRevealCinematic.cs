using System;
using System.Collections;
using LunarAnomaly.Player;
using LunarAnomaly.UI;
using TMPro;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class OutpostRevealCinematic : MonoBehaviour
    {
        [Header("References")]
        //[SerializeField] TerminalUpdateText updateText;
        [SerializeField] ScreenFader screenFader;
        [SerializeField] LayerMask playerLayer;
        // [SerializeField] TMP_Text currentTextBox;
		// [SerializeField] Typewriter typewriter;
        SpriteRenderer silhouette;

        [Header("Event Settings")]
        [SerializeField] float playerWatchingFOV = 0.6f;
        [SerializeField] float waitBeforeFlash = 2.5f;
        [SerializeField] float fadeBlackTime = 0.75f;
        [SerializeField] Transform playerTeleportPos;

        // [Header("Logs")]
		// [TextArea(5,10)]
		// [SerializeField] string logText;	

        // [SerializeField] GameObject demoCanvas;

        bool silhouetteActive;
        bool playerWatching;
        Camera cameraPos;

        // To PlayerState
        public static event Action<Transform, TeleportType> OnOutpostCinematicTeleport;
        // To OutpostController
        public static event Action OnDisableOutpost;
        // To SanityManager
        public static event Action OnActivateSanitySystem;
        // To DemoCanvas
        public static event Action OnDemoComplete;

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

            OnDemoComplete?.Invoke();
            

            // Silhouette.OnSilhouetteFlash?.Invoke();
            // OnOutpostCinematicTeleport?.Invoke(playerTeleportPos, TeleportType.Cinematic);
            // OnDisableOutpost?.Invoke();
            // silhouette.enabled = false;
            // OnActivateSanitySystem?.Invoke();

            // yield return new WaitForSeconds(fadeBlackTime);
            // Destroy(gameObject);
        }
    }
}

