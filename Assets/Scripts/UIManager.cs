using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Oxygen UI")]
    [SerializeField] GameObject oxygenUI50;
    [SerializeField] GameObject oxygenUI10;    
    [SerializeField] Image fadeToBlack;
    [SerializeField] Image oxygenBar;
    [SerializeField] float flashDuration = 2f;
    [SerializeField] float flashInterval = 0.25f;
    bool flashed50;
    bool flashed10;
    

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); <--- Gives me a warning
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void UpdateOxygenBar(float fillAmount)
    {
        oxygenBar.fillAmount = fillAmount;
    }

    public void CheckOxygenWarnings(float currentOxygen, float startingOxygen)
    {
        float percent = currentOxygen / startingOxygen;

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

    public void ResetOxygenWarnings()
    {
        flashed50 = false;
        flashed10 = false;

        Color color = fadeToBlack.color; // Clean this up
        color.a = 0f;
        fadeToBlack.color = color;
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

    public IEnumerator FadeToBlack(float oxygenGracePeriod)
    {
        float time = 0f;

        while (time < oxygenGracePeriod)
        {
            time += Time.deltaTime;
            float t = time / oxygenGracePeriod;

            Color color = fadeToBlack.color;
                                // alpha
            color.a = Mathf.Lerp(0, 1, t);
            fadeToBlack.color = color;

            yield return null;
        }
    }
}
