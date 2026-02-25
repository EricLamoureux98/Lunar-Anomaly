using System;
using Unity.Mathematics;
using UnityEngine;

namespace LunarAnomaly.Gameplay
{
    public class Rock : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ParticleSystem destructionParticlePrefab;
        [SerializeField] GameObject rockSamplePrefab;

        [Header("Rock settings")]
        [SerializeField] float shrinkRate = 0.98f;
        [SerializeField] float health;
        [SerializeField] float sampleDropChance = 0.3f;

        float currentHealth;
        bool isDestroyed;

        // Sent to pickaxe
        public static event Action<Rock> OnRockDestroyed;

        void Awake()
        {
            currentHealth = health;
        }

        public void DamageRock(float damage)
        {
            if (isDestroyed) return;

            currentHealth -= damage;
            ShrinkRock();
            //Debug.Log("Rock damaged, health: " + currentHealth);

            if (currentHealth <= 0)
            {
                DestroyRock();
            }
        }

        void DestroyRock()
        {
            if (isDestroyed) return; 
            isDestroyed = true;

            if (destructionParticlePrefab != null)
            {
                transform.localScale *= 1.1f;
                Instantiate(destructionParticlePrefab, transform.position, Quaternion.identity);
                RandomSampleSpawn();
            }
            
            // To pickaxe
            OnRockDestroyed?.Invoke(this);
            Destroy(gameObject); // Destroy after event to avoid errors
        }

        void RandomSampleSpawn()
        {
            // How do I remove the UnityEngine? 
            if (UnityEngine.Random.value < sampleDropChance)
            {
                Instantiate(rockSamplePrefab, transform.position, quaternion.identity);
            }
        }

        void ShrinkRock()
        {
            transform.localScale *= shrinkRate;
        }
    }
}
// Screen shake
// Time freeze when hit lands 
// Debris particles - done
// Sounds
// Scale down when being mined --- Stretch goal would be 3d models
