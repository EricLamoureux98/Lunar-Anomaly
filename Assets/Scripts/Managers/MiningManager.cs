using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class MiningManager : MonoBehaviour
    {
        [SerializeField] int debugSamplesCollected;
        //public int samplesRequired; // Make this better
        public int samplesCollected { get; private set; }

        // To UIManager - old
        // To ObjectiveManager                   
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
            //DebugSamplesCollected();
            //OnSamplesCarriedChanged?.Invoke(samplesCollected, samplesRequired);
        }

        void DebugSamplesCollected()
        {
            if (debugSamplesCollected > 0)
            {
                samplesCollected = debugSamplesCollected;
                //OnSamplesCarriedChanged?.Invoke(samplesCollected, samplesRequired);
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
}
// Different sample/rock types

