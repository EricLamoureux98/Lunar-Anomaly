using System;
using UnityEngine;

public class RockSample : MonoBehaviour
{
    // To MiningManager
    public static event Action OnRockSampleCollected;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            OnRockSampleCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
