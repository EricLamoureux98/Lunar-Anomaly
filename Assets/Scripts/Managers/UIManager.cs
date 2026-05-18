using System.Collections;
using TMPro;
using UnityEngine;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;

namespace LunarAnomaly.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ScreenFader screenFader;

        //[Header("Mining UI")]
        //[SerializeField] TextMeshProUGUI samplesCollectedText;

        [SerializeField] TextMeshProUGUI deathText;
        [SerializeField] CanvasGroup gameplayCanvasGroup;

        Coroutine currentEffectRoutine;

        void OnEnable()
        {
            PlayerState.OnPlayerDying += PlayerDying;
            //MiningManager.OnSamplesCarriedChanged += UpdateMiningSampleUI;
            Silhouette.OnSilhouetteFlash += SilhouetteFlash;
            PlayerState.OnHideGameplayUI += HideGameplayUI;
            GameManager.OnGameStateChanged += HandleGameStateChange;
            PlayerState.OnLadderTeleport += HandleLadderFlash;
        }

        void OnDisable()
        {
            PlayerState.OnPlayerDying -= PlayerDying;
            //MiningManager.OnSamplesCarriedChanged -= UpdateMiningSampleUI;
            Silhouette.OnSilhouetteFlash -= SilhouetteFlash;
            PlayerState.OnHideGameplayUI -= HideGameplayUI;
            GameManager.OnGameStateChanged -= HandleGameStateChange;
            PlayerState.OnLadderTeleport -= HandleLadderFlash;
        }

        void HandleGameStateChange(GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    
                    break;

                case GameState.Playing:
                    HideGameplayUI(false);
                    break;
                
                case GameState.GameOver:
                    HideGameplayUI(true);
                    GameOver();
                    break;
                
                case GameState.Paused:
                    HideGameplayUI(true); // Implement this
                    break;
            }
        }

        void HideGameplayUI(bool hidden)
        {
            if (hidden)
                gameplayCanvasGroup.alpha = 0f;
            else
                gameplayCanvasGroup.alpha = 1f;
        }

        void UpdateMiningSampleUI(int samples, int remaining)
        {
            //samplesCollectedText.text = string.Format("{0} / {1} collected", samples, remaining);
            //samplesCollectedText.text = string.Format("Samples collected: {0}", samples);
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
            //StartCoroutine(BlackScreenFlash(fadeLength));
            Debug.Log("Silhouette flash");
            PlayScreenEffect(ScreenEffectType.Silhouette);
        }

        // IEnumerator BlackScreenFlash(float duration)
        // {
        //     screenFader.StartFade(0f, 1f, 0.1f, false);
        //     yield return new WaitForSeconds(duration); // Adjust this for timing
        //     screenFader.StartFade(1f, 0f, 1.5f, false);
        // }

        void HandleLadderFlash()
        {
            //StartCoroutine(LadderFlash(duration));
            PlayScreenEffect(ScreenEffectType.LadderTeleport);
        }

        // IEnumerator LadderFlash(float duration)
        // {
        //     screenFader.StartFade(0f, 1f, 0.1f, false);
        //     yield return new WaitForSeconds(duration); // Adjust this for timing
        //     screenFader.StartFade(1f, 0f, 1.5f, false);
        // }

        void PlayerDying()
        {
            //StartCoroutine(DeathSequence(fadeLength));
            PlayScreenEffect(ScreenEffectType.Death);
        }

        // IEnumerator DeathSequence(float duration)
        // {
        //     screenFader.StartFade(0f, 1f, duration, false);
        //     yield return new WaitForSeconds(4f); // Adjust this for respawn timing
        //     screenFader.StartFade(1f, 0f, duration / 2, false);
        // }

        void GameOver()
        {
            // Prevents silhouette from interrupting
            // BE CAREFUL WITH THIS
            //StopAllCoroutines();
            //StartCoroutine(GameoverSequence());
            PlayScreenEffect(ScreenEffectType.GameOver);
        }

        // IEnumerator GameoverSequence()
        // {
        //     screenFader.StartFade(0f, 1f, 0.1f, true);
        //     yield return new WaitForSecondsRealtime(3f);
        //     deathText.gameObject.SetActive(true);
        // }
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
