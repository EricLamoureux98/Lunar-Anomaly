using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    //[SerializeField] MiningManager miningManager;
    [SerializeField] float health;
    float currentHealth;
    bool isDestroyed;

    public static event Action OnRockDestroyed;

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
        gameObject.SetActive(false);
        OnRockDestroyed?.Invoke();
    }
}

// Add breaking damage
