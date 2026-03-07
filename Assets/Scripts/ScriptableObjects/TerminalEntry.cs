using UnityEngine;

[System.Serializable]
public class TerminalEntry
{
    public int id; // Update this to an ENUM

    [TextArea(5,10)]
    public string text;
}
