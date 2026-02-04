using UnityEngine.UI;
using UnityEngine;
using System;

public class Oxygen : MonoBehaviour
{
    [SerializeField] Image oxygenBar;
    [SerializeField] float startingOxygen = 120f;
    [SerializeField] float drainRate = 1f;
    [SerializeField] float refillRate = 2f;

    [SerializeField] bool oxygenDraining = false;
    [SerializeField] bool oxygenRefilling = false;

    float currentOxygen; // public for debugging

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
        currentOxygen = startingOxygen;
    }

    void Update()
    {
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
        oxygenBar.fillAmount = currentOxygen / startingOxygen;

        if (currentOxygen <= 0) OxygenDepleted();
    }

    void RefillOxygen()
    {
        if (currentOxygen < startingOxygen)
        {
            currentOxygen += refillRate * Time.deltaTime;    
            oxygenBar.fillAmount = currentOxygen / startingOxygen;        
        }
    }

    void OxygenDepleted()
    {
        OnOxygenDepleted?.Invoke();
        //Debug.Log("Oxygen event sent to Player State");
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
}

// Future features

// Add warning when oxygen is half and at 10%
