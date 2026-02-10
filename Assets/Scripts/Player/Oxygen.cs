using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

public class Oxygen : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] Image oxygenBar; // This should eventually be moved to its own script
    [SerializeField] GameObject oxygenUI50; // These could be combined into a single TMP 
    [SerializeField] GameObject oxygenUI10;    
    [SerializeField] float flashDuration = 2f;
    [SerializeField] float flashInterval = 0.25f;

    [SerializeField] float startingOxygen = 120f;
    [SerializeField] float drainRate = 1f;
    [SerializeField] float refillRate = 2f;
    [SerializeField] bool oxygenDraining = false;
    [SerializeField] bool oxygenRefilling = false;

    float currentOxygen;
    bool oxygenActive;

    bool show50Oxy;
    bool show10Oxy;

    // Sent to PlayerState
    public static event Action OnOxygenDepleted; 

    void OnEnable()
    {
        AtmosphereTracker.OnPressurized += AtmosphereUpdated;
    }

    void OnDisable()
    {
        AtmosphereTracker.OnPressurized -= AtmosphereUpdated;
    }

    void Start()
    {
        oxygenActive = true;
        currentOxygen = startingOxygen;
    }

    void Update()
    {
        UpdateOxygen();        
    }

    void UpdateOxygen()
    {
        if (!oxygenActive) return; 
        OxygenLowWarning();

        if (oxygenDraining)
        {
            DrainOxygen();
        }

        if (oxygenRefilling)
        {
            RefillOxygen();
        }
    }

    void DrainOxygen()
    {
        currentOxygen -= drainRate * Time.deltaTime;
        UpdateOxygenBar(currentOxygen / startingOxygen);

        if (currentOxygen <= 0) OxygenDepleted();
    }

    void RefillOxygen()
    {
        if (currentOxygen < startingOxygen)
        {
            currentOxygen += refillRate * Time.deltaTime;          
            UpdateOxygenBar(currentOxygen / startingOxygen);
        }
    }

    void OxygenDepleted()
    {
        OnOxygenDepleted?.Invoke();
    }

    void AtmosphereUpdated(bool pressurized)
    {
        if (pressurized)
        {
            //Debug.Log("Player entered pressurized area");
            oxygenDraining = false;
            oxygenRefilling = true;
        }
        else
        {
            //Debug.Log("Player exited pressurized area");
            oxygenDraining = true;
            oxygenRefilling = false;
        }
    }

    void UpdateOxygenBar(float fillAmount)
    {
        oxygenBar.fillAmount = fillAmount;
    }

    void OxygenLowWarning()
    {
        if (!show50Oxy && currentOxygen <= startingOxygen / 2f)
        {
            show50Oxy = true; // <--- These need to be smarter... What if the oxygen does not reach 100%
            StartCoroutine(OxygenWarningFlash(oxygenUI50));
            //Debug.Log("Oxygen Half");
        }
        
        if (!show10Oxy && currentOxygen <= (startingOxygen * 0.1f))
        {
            show10Oxy = true;
            StartCoroutine(OxygenWarningFlash(oxygenUI10));
            //Debug.Log("Oxygen at 10%");
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
        }
    }
    

    public void SetActive(bool active)
    {
        oxygenActive = active;
    }

    public void ResetOxygen()
    {
        currentOxygen = startingOxygen; 
        UpdateOxygenBar(startingOxygen);
        show50Oxy = false;
        show10Oxy = false;
        oxygenActive = true;
    }
}

