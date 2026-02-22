using System;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    [SerializeField] int debugSamplesCollected;
    public int samplesCollected { get; private set; }

    // To UIManager                     
    public static event Action<int> OnSamplesCarriedChanged;

    void OnEnable()
    {
        RockSample.OnRockSampleCollected += SampleCollected;
    }

    void OnDisable()
    {
        RockSample.OnRockSampleCollected -= SampleCollected;
    }

    void Start()
    {
        samplesCollected = 0;
        OnSamplesCarriedChanged?.Invoke(samplesCollected);

        DebugSamplesCollected();
    }

    void DebugSamplesCollected()
    {
        if (debugSamplesCollected > 0)
        {
            samplesCollected = debugSamplesCollected;
            OnSamplesCarriedChanged?.Invoke(samplesCollected);
        }
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

