using UnityEngine;

public class Oxygen : MonoBehaviour
{
    [SerializeField] float startingOxygen = 120f;
    [SerializeField] float drainRate = 1f;
    [SerializeField] float refillRate = 2f;

    [SerializeField] bool oxygenDraining = false;
    [SerializeField] bool oxygenRefilling = false;

    public float currentOxygen; // public for debugging

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
            drainOxygen();
        }

        if (oxygenRefilling)
        {
            refillOxygen();
        }
    }

    void drainOxygen()
    {
        currentOxygen -= drainRate * Time.deltaTime;

        if (currentOxygen <= 0) oxygenDepleted();
    }

    void refillOxygen()
    {
        if (currentOxygen < startingOxygen)
        {
            currentOxygen += refillRate * Time.deltaTime;            
        }
    }

    void oxygenDepleted()
    {
        // Add visuals and sound 
        // Kill player after 0 oxygen for a short period

        gameObject.SetActive(false);
    }

    void AtmosphereUpdated(bool pressurized)
    {
        if (pressurized)
        {
            oxygenDraining = false;
            oxygenRefilling = true;
        }
        else
        {
            oxygenDraining = true;
            oxygenRefilling = false;
        }
    }
}

// Future features

// Add warning when oxygen is half and at 10%
