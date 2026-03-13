using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] Image fadeToBlack;
    Coroutine currentFade;
    
    public void StartFade(float startAlpha, float endAlpha, float duration)
    {
        if (currentFade != null)
        StopCoroutine(currentFade);

        currentFade = StartCoroutine(ScreenFade(startAlpha, endAlpha, duration));
    }

    void SetFadeAlpha(float alpha)
    {
        fadeToBlack.color = new Color(0f, 0f, 0f, alpha);
    }

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
