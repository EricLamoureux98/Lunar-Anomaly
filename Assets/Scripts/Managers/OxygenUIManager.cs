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
        bool flashed50;
        bool flashed10;
		Coroutine flashRoutine;

		void OnEnable()
        {
            Oxygen.OnOxygenChanged += UpdateOxygenUI;
            Oxygen.OnOxygenReset += ResetOxygenWarnings;
			HabitatAirlock.OnEnterAtmosphere += AtmosphereChangeWarning;
        }

        void OnDisable()
        {
            Oxygen.OnOxygenChanged -= UpdateOxygenUI;
            Oxygen.OnOxygenReset -= ResetOxygenWarnings;
			HabitatAirlock.OnEnterAtmosphere -= AtmosphereChangeWarning;
	}

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

                //StartCoroutine(OxygenWarningFlash(oxygenUI50));
				if (flashRoutine != null)
					StopCoroutine(flashRoutine);
				
				flashRoutine = StartCoroutine(OxygenWarningFlash(oxygenUI50));
            }

            if (!flashed10 && percent <= 0.1f)
            {
                flashed10 = true;

                //StartCoroutine(OxygenWarningFlash(oxygenUI10));
				if (flashRoutine != null)
					StopCoroutine(flashRoutine);
				
				flashRoutine = StartCoroutine(OxygenWarningFlash(oxygenUI10));
            }
        }

        void AtmosphereChangeWarning(bool inAtmosphere)
        {
            if (inAtmosphere)
            {
                //StartCoroutine(OxygenWarningFlash(enteringAtmosphere));
				if (flashRoutine != null)
					StopCoroutine(flashRoutine);
				
				flashRoutine = StartCoroutine(OxygenWarningFlash(enteringAtmosphere));
            }
            else
            {
                //StartCoroutine(OxygenWarningFlash(enteringVacuum));
				if (flashRoutine != null)
					StopCoroutine(flashRoutine);
				
				flashRoutine = StartCoroutine(OxygenWarningFlash(enteringVacuum));
            }
        }

        void ResetOxygenWarnings()
        {
            flashed50 = false;
            flashed10 = false;
			oxygenUI50.SetActive(false);
			oxygenUI10.SetActive(false);

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
	}
}
