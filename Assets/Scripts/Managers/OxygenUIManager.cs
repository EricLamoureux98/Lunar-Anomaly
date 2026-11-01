using System.Collections;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using UnityEngine;
using UnityEngine.UI;

namespace LunarAnomaly.UI
{
	public class OxygenUIManager : MonoBehaviour
	{
		[Header("Oxygen UI")]
        [SerializeField] GameObject oxygenUI50;
        [SerializeField] GameObject oxygenUI10;
        [SerializeField] GameObject enteringVacuum;
        [SerializeField] GameObject enteringAtmosphere;
        [SerializeField] Image oxygenBar;
        [SerializeField] float flashDuration = 2f;
        [SerializeField] float flashInterval = 0.25f;
        GameObject activeCanvas;
        bool flashed50;
        bool flashed10;
        
		Coroutine flashRoutine;

		void OnEnable()
        {
            Oxygen.OnOxygenChanged += UpdateOxygenUI;
            Oxygen.OnOxygenUIReset += ResetOxygenWarnings;
			HabitatAirlock.OnEnterAtmosphere += AtmosphereChangeWarning;
        }

        void OnDisable()
        {
            StopActiveWarning();
            Oxygen.OnOxygenChanged -= UpdateOxygenUI;
            Oxygen.OnOxygenUIReset -= ResetOxygenWarnings;
			HabitatAirlock.OnEnterAtmosphere -= AtmosphereChangeWarning;
	}

	void UpdateOxygenUI(float fillAmount)
        {
            oxygenBar.fillAmount = fillAmount;

            CheckOxygenWarnings(fillAmount);
        }

        void CheckOxygenWarnings(float percent)
        {
            if (!flashed10 && percent <= 0.1f)
            {
                flashed10 = true;
                flashed50 = true;
				
                SoundManager.PlaySound(SoundType.OxygenWarning, 1f);
                StartWarning(oxygenUI10);
            }        
            else if (!flashed50 && percent <= 0.5f)
            {
                flashed50 = true;
				
                SoundManager.PlaySound(SoundType.OxygenWarning, 1f);
                StartWarning(oxygenUI50);
            }
        }

        void AtmosphereChangeWarning(bool inAtmosphere)
        {
            if (inAtmosphere)
            {
                StartWarning(enteringAtmosphere);
            }
            else
            {
                StartWarning(enteringVacuum);
            }
        }

        void ResetOxygenWarnings()
        {
            flashed50 = false;
            flashed10 = false;
            StopActiveWarning();
        }

        void StartWarning(GameObject warningCanvas)
        {
            if (warningCanvas == activeCanvas) return;
            if (warningCanvas == null) return;
            if (flashRoutine != null) StopActiveWarning();

            activeCanvas = warningCanvas;

            flashRoutine = StartCoroutine(OxygenWarningFlash(activeCanvas));
        }

        void StopActiveWarning()
        {
            if (activeCanvas || flashRoutine != null)
            {
                if (activeCanvas != null)
                {
                    activeCanvas.SetActive(false);
                    activeCanvas = null;                
                }

                if (flashRoutine != null)
                {
                    StopCoroutine(flashRoutine);
                    flashRoutine = null;                
                }
            }
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
                activeCanvas = null;
            }

            flashRoutine = null;
        }
	}
}
