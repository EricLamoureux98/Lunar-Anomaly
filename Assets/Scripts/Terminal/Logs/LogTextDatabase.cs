using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LogTextDatabase", menuName = "Scriptable Objects/LogTextDatabase")]
public class LogTextDatabase : ScriptableObject
{
    public List<LogEntry> logEntries;

    public string GetLogText(LogMessage message)
    {
        foreach (var entry in logEntries)
        {
            if (entry.message == message)
            {
                return entry.logText;
            }
        }

        Debug.LogWarning($"Log message {message} not found in database!");
        return "Log message not found!";
    }

    public string GetLogTitle(string logTitle)
    {
        foreach (var entry in logEntries)
        {
            if (entry.logTitle == logTitle)
            {
                return entry.logTitle;
            }
        }

        Debug.LogWarning($"Log message {logTitle} not found in database!");
        return "Log title not found!";
    }
}
