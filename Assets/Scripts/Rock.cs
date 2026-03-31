using System;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LunarAnomaly.Gameplay
{
    public class Rock : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] ParticleSystem destructionParticlePrefab;
        //[SerializeField] GameObject rockSamplePrefab;
        [SerializeField] GameObject[] rockSamplePrefabs;

        [Header("Rock settings")]
        [SerializeField] Transform sampleSpawnPoint;
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

            SoundManager.PlaySound(SoundType.RockBreak, 0.4f);

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
            if (Random.value < sampleDropChance)
            {
                int index = Random.Range(0, rockSamplePrefabs.Length);
                Instantiate(rockSamplePrefabs[index], sampleSpawnPoint.position, quaternion.identity);
            }
        }

        void ShrinkRock()
        {
            transform.localScale *= shrinkRate;
        }
    }
}
// Screen shake - done
// Time freeze when hit lands 
// Debris particles - done
// Sounds - done
// Scale down when being mined --- Stretch goal would be 3d models
