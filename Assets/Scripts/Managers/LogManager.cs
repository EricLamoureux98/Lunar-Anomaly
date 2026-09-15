using System.Collections.Generic;
using UnityEngine;

public class LogManager : MonoBehaviour
{
    HashSet<LogMessage> discoveredLogs = new();

    public void DiscoverLog(LogMessage log)
    {
        Debug.Log($"Unlocked {log}");
        discoveredLogs.Add(log);
    }

    public bool IsDiscoverd(LogMessage log)
    {
        return discoveredLogs.Contains(log);
    }
}
