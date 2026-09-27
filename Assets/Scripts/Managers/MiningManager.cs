using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class MiningManager : MonoBehaviour
    {
        int requiredSamples;
        public int carriedSamples; // for testing
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
            HabitatDeposit.OnSamplesDeposited += DepositCarriedSamples;
            // HabitatController.OnDepositSamples += DepositCarriedSamples;
        }

        void OnDisable()
        {
            RockSample.OnRockSampleCollected -= SampleCollected;
            ObjectiveManager.OnBeginMiningObjective -= BeginMiningObjective;
            HabitatDeposit.OnSamplesDeposited -= DepositCarriedSamples;
            // HabitatController.OnDepositSamples -= DepositCarriedSamples;
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
            depositedSamples = Mathf.Min(depositedSamples, requiredSamples);

            carriedSamples = 0;

            OnDepositProgressChanged?.Invoke(depositedSamples, requiredSamples);
        }

        public void DebugUpdateSamplesCarried(int newCarried)
        {
            carriedSamples = newCarried;
        }
    }
}
// Different sample/rock types

