using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class MiningManager : MonoBehaviour
    {
        int requiredSamples;
        int carriedSamples;
        int depositedSamples;

        public int RequiredSamples => requiredSamples;
        public int CarriedSamples => carriedSamples;
        public int DepositedSamples => depositedSamples;

        // To ObjectiveManager                   
        public static event Action<int, int> OnDepositProgressChanged; // deposited, required

        void OnEnable()
        {
            RockSample.OnRockSampleCollected += SampleCollected;
            ObjectiveManager.OnBeginMiningObjective += BeginMiningObjective;
            HabitatController.OnDepositSamples += DepositCarriedSamples;
        }

        void OnDisable()
        {
            RockSample.OnRockSampleCollected -= SampleCollected;
            ObjectiveManager.OnBeginMiningObjective -= BeginMiningObjective;
            HabitatController.OnDepositSamples -= DepositCarriedSamples;
        }

        void BeginMiningObjective(int required)
        {
            requiredSamples = required;
            carriedSamples = 0;
            depositedSamples = 0;
        }

        void SampleCollected()
        {
            carriedSamples++;
        }

        void DepositCarriedSamples()
        {
            if (carriedSamples <= 0) return;
            
            depositedSamples += carriedSamples;
            carriedSamples = 0;

            OnDepositProgressChanged?.Invoke(depositedSamples, requiredSamples);
        }
    }
}
// Different sample/rock types

