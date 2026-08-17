using System;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class RockSample : MonoBehaviour
    {
        // To MiningManager
        public static event Action OnRockSampleCollected;

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                SoundManager.PlaySound(SoundType.Pickup, 1f);
                OnRockSampleCollected?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}