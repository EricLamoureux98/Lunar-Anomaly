using System;
using UnityEngine;

public class LogPickup : MonoBehaviour
{
    [SerializeField] LogMessage log;
    [SerializeField] LogManager logManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectLog();
            
            Debug.Log($"Log {log} picked up");
        }
    }

    void CollectLog()
    {
        logManager.DiscoverLog(log);
        Destroy(gameObject);
    }
}
