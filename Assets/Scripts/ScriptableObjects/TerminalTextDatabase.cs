using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerminalTextDatabase", menuName = "Scriptable Objects/TerminalTextDatabase")]
public class TerminalTextDatabase : ScriptableObject
{
    public List<TerminalEntry> entries;

    public string GetText(int id)
    {
        foreach (var entry in entries)
        {
            if (entry.id == id)
            {
                return entry.text;
            }
        }
        //return "";
        return "Text not found!";
    }
}
