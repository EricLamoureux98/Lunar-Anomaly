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
        [Header("Oxygen UI")]
        [SerializeField] GameObject oxygenUI50;
        [SerializeField] GameObject oxygenUI10;
        [SerializeField] Image fadeToBlack;
        [SerializeField] Image oxygenBar;
        [SerializeField] float flashDuration = 2f;
        [SerializeField] float flashInterval = 0.25f;
        bool flashed50;
        bool flashed10;
        Coroutine currentFade;

        [Header("Mining UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;

        void OnEnable()
        {
            Oxygen.OnOxygenChanged += UpdateOxygenUI;
            Oxygen.OnOxygenReset += ResetOxygenWarnings;
            PlayerState.OnPlayerDying += PlayerDying;
            MiningManager.OnSamplesCarriedChanged += UpdateMiningSampleUI;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenChanged -= UpdateOxygenUI;
            Oxygen.OnOxygenReset -= ResetOxygenWarnings;
            PlayerState.OnPlayerDying -= PlayerDying;
            MiningManager.OnSamplesCarriedChanged -= UpdateMiningSampleUI;
        }

        void UpdateMiningSampleUI(int samples)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples collected: {0}", samples);
        }

        // Oxygen UI might be worth creating a new script
        void UpdateOxygenUI(float fillAmount)
        {
            oxygenBar.fillAmount = fillAmount;

            CheckOxygenWarnings(fillAmount);
        }

        void CheckOxygenWarnings(float percent)
        {

            if (!flashed50 && percent <= 0.5f)
            {
                flashed50 = true;
                StartCoroutine(OxygenWarningFlash(oxygenUI50));
            }

            if (!flashed10 && percent <= 0.1f)
            {
                flashed10 = true;
                StartCoroutine(OxygenWarningFlash(oxygenUI10));
            }
        }

        void ResetOxygenWarnings()
        {
            flashed50 = false;
            flashed10 = false;

            //SetFadeAlpha(0f);
            oxygenBar.fillAmount = 1f;
        }

        IEnumerator OxygenWarningFlash(GameObject canvas)
        {
            float timer = 0f;

            while (timer < flashDuration)
            {
                if (canvas != null)
                {
                    canvas.SetActive(!canvas.activeSelf);
                }

                yield return new WaitForSeconds(flashInterval);
                timer += flashInterval;
            }

            if (canvas != null)
            {
                canvas.SetActive(false);
            }
        }

        void SetFadeAlpha(float alpha)
        {
            fadeToBlack.color = new Color(0f, 0f, 0f, alpha);
        }

        void PlayerDying(float fadeLength)
        {
            StartCoroutine(DeathSequence(fadeLength));
        }

        IEnumerator DeathSequence(float duration)
        {
            StartFade(0f, 1f, duration);
            yield return new WaitForSeconds(4f); // Adjust this for respawn timing
            StartFade(1f, 0f, duration / 2);
        }

        void StartFade(float startAlpha, float endAlpha, float duration)
        {
            if (currentFade != null)
            StopCoroutine(currentFade);

            currentFade = StartCoroutine(ScreenFade(startAlpha, endAlpha, duration));
        }

        // Consider making this its own class - ScreenFader
        IEnumerator ScreenFade(float startAlpha, float endAlpha, float duration)
        {
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                float t = time / duration;

                SetFadeAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

                yield return null;
            }

            SetFadeAlpha(endAlpha);
        }
    }
}
