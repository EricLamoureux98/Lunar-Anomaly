using System.Collections;
using UnityEngine;
using TMPro;
using LunarAnomaly.Player;
using LunarAnomaly.Gameplay;

namespace LunarAnomaly.UI
{
    public class ScreenEffect : MonoBehaviour
    {
        [SerializeField] ScreenFader screenFader;

        Coroutine currentEffectRoutine;

        void OnEnable()
        {
            PlayerState.OnPlayerDying += PlayerDying;
            Silhouette.OnSilhouetteFlash += SilhouetteFlash;
            PlayerState.OnLadderTeleport += HandleLadderFlash;
        }

        void OnDisable()
        {
            PlayerState.OnPlayerDying -= PlayerDying;
            Silhouette.OnSilhouetteFlash -= SilhouetteFlash;
            PlayerState.OnLadderTeleport -= HandleLadderFlash;
        }

        public void PlayScreenEffect(ScreenEffectType effectType)
        {
            // Consider adding priority effects
            if (currentEffectRoutine != null)
            {
                StopCoroutine(currentEffectRoutine);
            }
            
            switch (effectType)
            {
                case ScreenEffectType.StayBlack:
                    currentEffectRoutine = StartCoroutine(ScreenToBlackRoutine());
                    break;

                case ScreenEffectType.ToWhite:
                    currentEffectRoutine = StartCoroutine(ScreenBlinkWhiteRoutine());
                    break;
                
                default:
                    currentEffectRoutine = StartCoroutine(ScreenBlinkBlackRoutine(effectType));
                    break;
            }
        }

        IEnumerator ScreenBlinkBlackRoutine(ScreenEffectType effectType)
        {
            float fadeInDuration = 0.1f;
            float holdDuration = 0.5f;
            float fadeOutDuration = 1f;
            bool useRealtime = false;

            switch (effectType)
            {
                case ScreenEffectType.Death:
                    fadeInDuration = 1f;
                    holdDuration = 3f;
                    fadeOutDuration = 1.5f;
                    break;

                case ScreenEffectType.ShortBlink:
                    fadeInDuration = 0.1f;
                    holdDuration = 0.15f;
                    fadeOutDuration = 0.5f;
                    break;

                case ScreenEffectType.LadderTeleport:
                    holdDuration = 0.25f;
                    fadeOutDuration = 1.5f;
                    break;
                
                case ScreenEffectType.Cinematic:
                    holdDuration = 5f;
                    fadeOutDuration = 2f;
                    break;
            }

            screenFader.StartFadeToBlack(0f, 1f, fadeInDuration, useRealtime);

            yield return useRealtime ? new WaitForSecondsRealtime(holdDuration) : new WaitForSeconds(holdDuration);

            screenFader.StartFadeToBlack(1f, 0f, fadeOutDuration, useRealtime);

            yield return new WaitForSeconds(fadeOutDuration);
            currentEffectRoutine = null;
        }

        IEnumerator ScreenBlinkWhiteRoutine()
        {
            float fadeInDuration = 0.1f;
            float holdDuration = 0.7f;
            float fadeOutDuration = 0.5f;
            bool useRealtime = false;

            screenFader.StartFadeToWhite(0f, 1f, fadeInDuration, useRealtime);

            yield return useRealtime ? new WaitForSecondsRealtime(holdDuration) : new WaitForSeconds(holdDuration);

            screenFader.StartFadeToWhite(1f, 0f, fadeOutDuration, useRealtime);

            yield return new WaitForSeconds(fadeOutDuration);
            currentEffectRoutine = null;
        }

        IEnumerator ScreenToBlackRoutine()
        {
            float fadeInDuration = 0.35f;
            bool useRealtime = false;

            screenFader.StartFadeToBlack(0f, 1f, fadeInDuration, useRealtime);

            yield return new WaitForSeconds(fadeInDuration);

            currentEffectRoutine = null;
        }

        void SilhouetteFlash()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            Debug.Log("Silhouette flash");
            PlayScreenEffect(ScreenEffectType.ShortBlink);
        }

        void HandleLadderFlash()
        {
            PlayScreenEffect(ScreenEffectType.LadderTeleport);
        }

        void PlayerDying()
        {
            PlayScreenEffect(ScreenEffectType.Death);
        }
    }
}

public enum ScreenEffectType
{
    Death,
    ShortBlink,
    Cinematic,
    LadderTeleport,
    StayBlack,
    ToWhite
}

// Made add later
// public enum ScreenEffectPriority
// {
//     Low,
//     Medium,
//     High,
//     Critical
// }