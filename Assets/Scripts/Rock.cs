using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    //[SerializeField] MiningManager miningManager;
    [SerializeField] float health;
    float currentHealth;
    bool isDestroyed;

    public static event Action<Rock> OnRockDestroyed;

    void Awake()
    {
        currentHealth = health;
    }

    public void DamageRock(float damage)
    {
        if (isDestroyed) return;

        currentHealth -= damage;
        Debug.Log("Rock damaged, health: " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyRock();
        }
    }

    void DestroyRock()
    {
        if (isDestroyed) return; 

        isDestroyed = true;
        OnRockDestroyed?.Invoke(this);
        Destroy(gameObject); // Destroy after event to avoid errors
    }
}

// Screen shake
// Time freeze when hit lands 
// Debris particles
// Sounds
// Scale down when being mined --- Stretch goal would be 3d models
