using System;
using UnityEngine;

public class LogPickup : MonoBehaviour
{
    [SerializeField] LogMessage log;
    [SerializeField] LogTextDatabase logTextDatabase;

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
        logTextDatabase.DiscoverLog(log);
        Destroy(gameObject);
    }
}
