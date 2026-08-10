using System;
using System.Collections;
using LunarAnomaly.Player;
using LunarAnomaly.UI;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class OutpostRevealCinematic : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineCamera virtualCamera;
        [SerializeField] ScreenFader screenFader;
        [SerializeField] LayerMask playerLayer;
        [SerializeField] Transform HabitatWaypoint;
        [SerializeField] DiscoveryZone habitatDiscoveryZone;
        [SerializeField] Transform hitbox;
        SpriteRenderer silhouette;

        [Header("Event Settings")]
        [SerializeField] float playerWatchingFOV = 0.6f;
        [SerializeField] float waitBeforeFlash = 2.5f;
        [SerializeField] float fadeBlackTime = 0.75f;
        [SerializeField] float defaultCamFOV = 70f;
        [SerializeField] float cinematicCamFOV = 20f;
        [SerializeField] float camZoomSpeed = 10f;
        [SerializeField] Transform playerTeleportPos;

        bool silhouetteActive;
        bool playerWatching;
        bool camZooming;
        Camera cam;

        // To PlayerState
        public static event Action<Transform, TeleportType> OnOutpostCinematicTeleport;
        // To OutpostController
        public static event Action OnDisableOutpost;
        // To SanityManager
        public static event Action OnActivateSanitySystem;
        // To DemoCanvas
        //public static event Action OnDemoComplete;
        // To PlayerLook
        public static event Action<bool> OnSilhouetteSensitivity;
        // To ProgressionManager
        public static event Action OnOutpostMissionComplete;
        // To WaypointManager
        public static event Action<Transform> OnUpdateWaypointTarget;
        public static event Action<bool> OnUpdateWaypointActive;

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
            cam = Camera.main;
        }

        void Update()
        {
            if (silhouetteActive)
                CheckSilhouetteSeen();

            if (camZooming)
            {
                ZoomIn();
            }
        }

        void ShowSilhouette()
        {
            silhouetteActive = true;
            silhouette.enabled = true;
        }

        void CheckSilhouetteSeen()
        {
            playerWatching = PlayerVision.IsPointVisible(cam, hitbox, playerWatchingFOV, playerLayer);

            if (playerWatching)
            {
                silhouetteActive = false;

                OnSilhouetteSensitivity?.Invoke(true);
                camZooming = true;
                StartCoroutine(SilhouetteEvent());
            }
        }

        void ZoomIn()
        {
            float current = virtualCamera.Lens.FieldOfView;

            float next = Mathf.MoveTowards(current, cinematicCamFOV, camZoomSpeed * Time.deltaTime);

            virtualCamera.Lens.FieldOfView = next;
        }

        void ResetZoom()
        {
            camZooming = false;
            virtualCamera.Lens.FieldOfView = defaultCamFOV;
        }

        IEnumerator SilhouetteEvent()
        {           
            SoundManager.PlaySound(SoundType.AlienSeenFirstTime, 2.5f, false);
            SoundManager.PlaySound(SoundType.Heartbeat, 3f, false);

            yield return new WaitForSeconds(waitBeforeFlash);

            //OnDemoComplete?.Invoke();

            OnSilhouetteSensitivity?.Invoke(false);
            ResetZoom();
            

            Silhouette.OnSilhouetteFlash?.Invoke();
            
            OutpostController.OnOutpostAdvanced?.Invoke(ProgressionStage.OutpostObjective);
            OnOutpostCinematicTeleport?.Invoke(playerTeleportPos, TeleportType.Cinematic);
            OnDisableOutpost?.Invoke();
            silhouette.enabled = false;
            OnActivateSanitySystem?.Invoke();

            yield return new WaitForSeconds(fadeBlackTime);
            
            // --- OUTPOST COMPLETE ---
            TerminalUI.OnRequestNotification?.Invoke(NotificationMessage.OutpostPowerOn);
            TerminalUI.OnRequestNotificationDelayed?.Invoke(NotificationMessage.OutpostTransmission, 8f);

            TerminalUI.OnRequestNotificationDelayed?.Invoke(NotificationMessage.ReturnToHabitat, 14f);

            yield return new WaitForSeconds(19f);

            OnOutpostMissionComplete?.Invoke();
            OnUpdateWaypointTarget?.Invoke(HabitatWaypoint);
            OnUpdateWaypointActive?.Invoke(true);
            habitatDiscoveryZone.ChangeActive(true);

            Destroy(gameObject);
        }
    }
}

