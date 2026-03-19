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

        [Header("Mining UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;

        [SerializeField] TextMeshProUGUI deathText;
        [SerializeField] CanvasGroup canvasGroup;

        void OnEnable()
        {
            PlayerState.OnPlayerDying += PlayerDying;
            MiningManager.OnSamplesCarriedChanged += UpdateMiningSampleUI;
            Silhouette.OnSilhouetteFlash += SilhouetteFlash;
            PlayerState.OnHideGameplayUI += HideGameplayUI;
            PlayerState.OnTriggerGameOver += GameOver;
        }

        void OnDisable()
        {
            PlayerState.OnPlayerDying -= PlayerDying;
            MiningManager.OnSamplesCarriedChanged -= UpdateMiningSampleUI;
            Silhouette.OnSilhouetteFlash -= SilhouetteFlash;
            PlayerState.OnHideGameplayUI -= HideGameplayUI;
            PlayerState.OnTriggerGameOver -= GameOver;
        }

        void HideGameplayUI(bool hidden)
        {
            if (hidden)
                canvasGroup.alpha = 0f;
            else
                canvasGroup.alpha = 1f;
        }

        void UpdateMiningSampleUI(int samples)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples collected: {0}", samples);
        }

        void SilhouetteFlash(float fadeLength)
        {
            StartCoroutine(BlackScreenFlash(fadeLength));
        }

        IEnumerator BlackScreenFlash(float duration)
        {
            screenFader.StartFade(0f, 1f, 0.1f);
            yield return new WaitForSeconds(duration); // Adjust this for timing
            screenFader.StartFade(1f, 0f, 1.5f);
        }

        void PlayerDying(float fadeLength)
        {
            StartCoroutine(DeathSequence(fadeLength));
        }

        IEnumerator DeathSequence(float duration)
        {
            screenFader.StartFade(0f, 1f, duration);
            yield return new WaitForSeconds(4f); // Adjust this for respawn timing
            screenFader.StartFade(1f, 0f, duration / 2);
        }

        void GameOver()
        {
            StartCoroutine(GameoverSequence());
        }

        IEnumerator GameoverSequence()
        {
            screenFader.StartFade(0f, 1f, 0.1f);
            yield return new WaitForSeconds(3f);
            deathText.gameObject.SetActive(true);
        }
    }
}
