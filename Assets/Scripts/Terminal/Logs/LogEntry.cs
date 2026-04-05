using UnityEngine;

[System.Serializable]
public class LogEntry
{
	public LogMessage message;

	public string logTitle;

	[TextArea(5,10)]
	public string logText;

	public bool isDiscovered;
}
