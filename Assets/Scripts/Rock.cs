using System;
using UnityEngine;

public class Rock : MonoBehaviour
{
    //[SerializeField] MiningManager miningManager;
    [SerializeField] float health;
    float currentHealth;

    public static event Action OnRockDestroyed;

    void Awake()
    {
        currentHealth = health;
    }

    public void DamageRock(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Rock damaged, health: " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyRock();
        }
    }

    void DestroyRock()
    {
        gameObject.SetActive(false);
        OnRockDestroyed?.Invoke();
    }
}
