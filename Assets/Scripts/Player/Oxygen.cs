using UnityEngine.UI;
using UnityEngine;
using System;

public class Oxygen : MonoBehaviour
{   
    [SerializeField] float startingOxygen = 120f;
    [SerializeField] float drainRate = 1f;
    [SerializeField] float refillRate = 2f;
    [SerializeField] bool oxygenDraining = false;
    [SerializeField] bool oxygenRefilling = false;

    float currentOxygen;
    bool oxygenActive;

    // Sent to PlayerState
    public static event Action OnOxygenDepleted; 

    // Add eventually -> UIManager
    //public static event Action<float> OnOxygenChanged;

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
        UIManager.Instance.UpdateOxygenBar(currentOxygen / startingOxygen);

        if (currentOxygen <= 0) OxygenDepleted();
    }

    void RefillOxygen()
    {
        if (currentOxygen < startingOxygen)
        {
            currentOxygen += refillRate * Time.deltaTime;          
            UIManager.Instance.UpdateOxygenBar(currentOxygen / startingOxygen);
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

    void OxygenLowWarning()
    {
        if (currentOxygen <= startingOxygen / 2f)
        {
            UIManager.Instance.CheckOxygenWarnings(currentOxygen, startingOxygen);
        }
        
        if (currentOxygen <= (startingOxygen * 0.1f))
        {
            UIManager.Instance.CheckOxygenWarnings(currentOxygen, startingOxygen);
        }
    }    

    public void SetActive(bool active)
    {
        oxygenActive = active;
    }

    public void ResetOxygen()
    {
        currentOxygen = startingOxygen; 
        UIManager.Instance.UpdateOxygenBar(startingOxygen);
        UIManager.Instance.ResetOxygenWarnings();
        oxygenActive = true;
    }
}

