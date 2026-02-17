using System;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    //[SerializeField] GameObject[] rocks;
    [SerializeField] int samplesRequired;

    int samplesCollected;

    // To UIManager                     // Consider updating this name
    public static event Action<int, int> OnSampleObjectiveUpdate;

    //float rocksRemaining;

    void OnValidate()
    {
        //rocksRemaining = rocks.Length;
        //Debug.Log("Rocks remaining: " + rocksRemaining);
    }

    void OnEnable()
    {
        //Rock.OnRockDestroyed += RockDestroyed;
        RockSample.OnRockSampleCollected += SampleCollected;
    }

    void OnDisable()
    {
        //Rock.OnRockDestroyed -= RockDestroyed;
        RockSample.OnRockSampleCollected -= SampleCollected;
    }

    void Start()
    {
        //rocksRemaining = rocks.Length;
        OnSampleObjectiveUpdate?.Invoke(samplesCollected, samplesRequired);
        samplesCollected = 0;
    }

    // void RockDestroyed(Rock rock)
    // {
    //     rocksRemaining--;
    //     Debug.Log("Rocks remaining: " + rocksRemaining);
    // }   

    void SampleCollected()
    {
        if (samplesCollected < samplesRequired)
        {
            samplesCollected++;
            OnSampleObjectiveUpdate?.Invoke(samplesCollected, samplesRequired);
        }
    }
}

// Maybe different rock types

