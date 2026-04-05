using UnityEngine;

[System.Serializable]
public class TerminalEntry
{
    public TerminalMessage message;

    [TextArea(5,10)]
    public string text;
}
