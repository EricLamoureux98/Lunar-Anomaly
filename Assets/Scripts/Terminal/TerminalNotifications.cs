using System;
using LunarAnomaly.Gameplay;
using LunarAnomaly.UI;
using TMPro;
using UnityEngine;

public class TerminalNotifications : MonoBehaviour
{
    [SerializeField] TerminalUI terminalUI;
    //[SerializeField] TerminalMessage notification;
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] Transform notificationContainer;

    void OnEnable()
    {
        HabitatAirlock.OnAirlockCycled += CreateAirlockNotification;
    }

    void OnDisable()
    {
        HabitatAirlock.OnAirlockCycled -= CreateAirlockNotification;
    }

    void Start()
    {
        //CreateAirlockNotification();
    }

    void CreateAirlockNotification()
    {
        notificationPrefab = Instantiate(notificationPrefab, notificationContainer);
        TextMeshProUGUI text = notificationPrefab.GetComponentInChildren<TextMeshProUGUI>();
        
        string currentTime;

        if (terminalUI.CurrentTime == "")
            currentTime = "00:00:00";
        else
            currentTime = terminalUI.CurrentTime;        

        string notifText = $"[SOL 028 ' {currentTime}] Airlock Cycle complete. Outer door sealed.";
        
        text.text = notifText;
    }
}
