using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;

namespace LunarAnomaly.UI
{
    // Disable overlay UI when in terminal
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ScreenFader screenFader;

        //[Header("Mining UI")]
        //[SerializeField] TextMeshProUGUI samplesCollectedText;

        [SerializeField] TextMeshProUGUI deathText;
        [SerializeField] CanvasGroup gameplayCanvasGroup;

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
                    // Show main menu canvas
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

        void SilhouetteFlash(float fadeLength)
        {
            if (GameManager.Instance.CurrentState != GameState.Playing) return;
            StartCoroutine(BlackScreenFlash(fadeLength));
        }

        IEnumerator BlackScreenFlash(float duration)
        {
            screenFader.StartFade(0f, 1f, 0.1f, false);
            yield return new WaitForSeconds(duration); // Adjust this for timing
            screenFader.StartFade(1f, 0f, 1.5f, false);
        }

        void HandleLadderFlash(float duration)
        {
            StartCoroutine(LadderFlash(duration));
        }

        IEnumerator LadderFlash(float duration)
        {
            screenFader.StartFade(0f, 1f, 0.1f, false);
            yield return new WaitForSeconds(duration); // Adjust this for timing
            screenFader.StartFade(1f, 0f, 1.5f, false);
        }

        void PlayerDying(float fadeLength)
        {
            StartCoroutine(DeathSequence(fadeLength));
        }

        IEnumerator DeathSequence(float duration)
        {
            screenFader.StartFade(0f, 1f, duration, false);
            yield return new WaitForSeconds(4f); // Adjust this for respawn timing
            screenFader.StartFade(1f, 0f, duration / 2, false);
        }

        void GameOver()
        {
            // Prevents silhouette from interrupting
            // BE CAREFUL WITH THIS
            StopAllCoroutines();
            StartCoroutine(GameoverSequence());
        }

        IEnumerator GameoverSequence()
        {
            screenFader.StartFade(0f, 1f, 0.1f, true);
            yield return new WaitForSecondsRealtime(3f);
            deathText.gameObject.SetActive(true);
        }
    }
}
