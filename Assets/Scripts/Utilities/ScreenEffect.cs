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
        [SerializeField] TextMeshProUGUI deathText;

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
            if (currentEffectRoutine != null)
            {
                StopCoroutine(currentEffectRoutine);
            }
            
            currentEffectRoutine = StartCoroutine(ScreenEffectRoutine(effectType));
        }

        IEnumerator ScreenEffectRoutine(ScreenEffectType effectType)
        {
            float fadeInDuration = 0.1f;
            float holdDuration = 0.5f;
            float fadeOutDuration = 1f;
            bool useRealtime = false;

            switch (effectType)
            {
                case ScreenEffectType.Death:
                    fadeInDuration = 3f;
                    holdDuration = 1f;
                    fadeOutDuration = 1.5f;
                    break;

                case ScreenEffectType.GameOver:
                    holdDuration = 3f;
                    useRealtime = true;
                    break;

                case ScreenEffectType.Silhouette:
                    holdDuration = 0.25f;
                    break;

                case ScreenEffectType.LadderTeleport:
                    holdDuration = 0.25f;
                    fadeOutDuration = 1.5f;
                    break;
            }

            screenFader.StartFade(0f, 1f, fadeInDuration, useRealtime);

            yield return useRealtime ? new WaitForSecondsRealtime(holdDuration) : new WaitForSeconds(holdDuration);

            if (effectType == ScreenEffectType.GameOver)
            {
                deathText.gameObject.SetActive(true);
                yield break;
            }

            screenFader.StartFade(1f, 0f, fadeOutDuration, useRealtime);
        }

        void SilhouetteFlash()
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;

            Debug.Log("Silhouette flash");
            PlayScreenEffect(ScreenEffectType.Silhouette);
        }

        void HandleLadderFlash()
        {
            PlayScreenEffect(ScreenEffectType.LadderTeleport);
        }

        void PlayerDying()
        {
            PlayScreenEffect(ScreenEffectType.Death);
        }

        public void GameOver()
        {
            PlayScreenEffect(ScreenEffectType.GameOver);
        }
    }
}

public enum ScreenEffectType
{
    Death,
    GameOver,
    Silhouette,
    Cinematic,
    LadderTeleport
}

