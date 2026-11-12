using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] Image fadeToBlack;
    [SerializeField] Image fadeToWhite;

    Image colorToFade;
    Coroutine currentFade;

    public void StartFadeToBlack(float startAlpha, float endAlpha, float duration, bool useUnscaledTime)
    {
        if (currentFade != null)
            CancelActiveFade();

        colorToFade = fadeToBlack;

        currentFade = StartCoroutine(ScreenFade(startAlpha, endAlpha, duration, useUnscaledTime));
    }

    public void StartFadeToWhite(float startAlpha, float endAlpha, float duration, bool useUnscaledTime)
    {
        if (currentFade != null)
            CancelActiveFade();

        colorToFade = fadeToWhite;

        currentFade = StartCoroutine(ScreenFade(startAlpha, endAlpha, duration, useUnscaledTime));
    }

    void SetFadeAlpha(float alpha)
    {
        Color color = colorToFade.color;
        color.a = alpha;
        colorToFade.color = color;
    }

    IEnumerator ScreenFade(float startAlpha, float endAlpha, float duration, bool useUnscaledTime)
    {
        float time = 0f;

        SetFadeAlpha(startAlpha);

        while (time < duration)
        {
            // Unscaled can be used if Time.timeScale = 0
            time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

            float t = time / duration;

            SetFadeAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            yield return null;
        }

        SetFadeAlpha(endAlpha);
        currentFade = null;
    }

    void CancelActiveFade()
    {
        StopCoroutine(currentFade);
        SetFadeAlpha(0f);
        currentFade = null;
    }
}
