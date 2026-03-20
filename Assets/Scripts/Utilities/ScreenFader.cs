using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] Image fadeToBlack;
    Coroutine currentFade;
    
    public void StartFade(float startAlpha, float endAlpha, float duration, bool useUnscaledTime)
    {
        if (currentFade != null)
        StopCoroutine(currentFade);

        currentFade = StartCoroutine(ScreenFade(startAlpha, endAlpha, duration, useUnscaledTime));
    }

    void SetFadeAlpha(float alpha)
    {
        fadeToBlack.color = new Color(0f, 0f, 0f, alpha);
    }

    IEnumerator ScreenFade(float startAlpha, float endAlpha, float duration, bool useUnscaledTime)
    {
        float time = 0f;

        while (time < duration)
        {
            // Unscaled can be used if Time.timeScale = 0
            time += useUnscaledTime? Time.unscaledDeltaTime : Time.deltaTime;

            float t = time / duration;

            SetFadeAlpha(Mathf.Lerp(startAlpha, endAlpha, t));

            yield return null;
        }

        SetFadeAlpha(endAlpha);
    }
}
