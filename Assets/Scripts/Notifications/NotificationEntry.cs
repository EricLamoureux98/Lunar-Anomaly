using UnityEngine;

[System.Serializable]
public class NotificationEntry
{
    public NotificationMessage message;

    [TextArea(5,10)]
    public string notificationText;
}
