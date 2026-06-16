using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NotificationDatabase", menuName = "Scriptable Objects/NotificationDatabase")]
public class NotificationDatabase : ScriptableObject
{
    public List<NotificationEntry> notificationEntries;

    public string GetNotificationText(NotificationMessage message)
    {
        foreach (var entry in notificationEntries)
        {
            if (entry.message == message)
            {
                return entry.notificationText;
            }
        }

        Debug.LogWarning($"Notification message {message} not found in database!");
        return "Notification message not found!";
    }
}
