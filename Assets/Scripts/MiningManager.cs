using System;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public int samplesCollected { get; private set; }

    // To UIManager                     
    public static event Action<int> OnSamplesCarriedChanged;

    void OnEnable()
    {
        RockSample.OnRockSampleCollected += SampleCollected;
    }

    void OnDisable()
    {
        //Rock.OnRockDestroyed -= RockDestroyed;
        RockSample.OnRockSampleCollected -= SampleCollected;
    }

    void Start()
    {
        samplesCollected = 0;
        OnSamplesCarriedChanged?.Invoke(samplesCollected);
    }

    void SampleCollected()
    {
        samplesCollected++;
        OnSamplesCarriedChanged?.Invoke(samplesCollected);
    }

    public void ClearSamples()
    {
        samplesCollected = 0;
        OnSamplesCarriedChanged?.Invoke(samplesCollected);
    }
}

// Different sample/rock types

