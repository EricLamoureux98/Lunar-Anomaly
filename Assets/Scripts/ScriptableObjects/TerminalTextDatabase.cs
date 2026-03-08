using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerminalTextDatabase", menuName = "Scriptable Objects/TerminalTextDatabase")]
public class TerminalTextDatabase : ScriptableObject
{
    public List<TerminalEntry> entries;

    public string GetText(TerminalMessage message)
    {
        foreach (var entry in entries)
        {
            if (entry.message == message)
            {
                return entry.text;
            }
        }
        
        Debug.LogWarning($"Terminal message {message} not found in database!");
        return "Termianl message not found!";
    }
}
