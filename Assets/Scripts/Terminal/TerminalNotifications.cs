using System.Collections;
using LunarAnomaly.Gameplay;
using LunarAnomaly.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TerminalNotifications : MonoBehaviour
{
    [SerializeField] TerminalUI terminalUI;
    //[SerializeField] TerminalMessage notification;
    [SerializeField] GameObject notificationPrefab;
    [SerializeField] Transform notificationContainer;
    [SerializeField] GameObject cursor;

    private Coroutine blinkCoroutine;

    void OnEnable()
    {
        HabitatAirlock.OnAirlockCycled += CreateAirlockNotification;
        TerminalInterfacePanel.OnStartingAirlockNotification += CreateAirlockNotification;
        TerminalInterfacePanel.OnViewNotification += SetActive;
    }

    void OnDisable()
    {
        HabitatAirlock.OnAirlockCycled -= CreateAirlockNotification;
        TerminalInterfacePanel.OnStartingAirlockNotification -= CreateAirlockNotification;
        TerminalInterfacePanel.OnViewNotification -= SetActive;
    }

    void SetActive(bool active)
    {
        if (blinkCoroutine != null) return;

        if (active)
            blinkCoroutine = StartCoroutine(CursorBlink());
        else 
            StopCoroutine(blinkCoroutine);
    }

    void Start()
    {
        //CreateAirlockNotification();
        cursor.transform.SetAsLastSibling();
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

        string notifText = $"[SOL 001 ' {currentTime}] Airlock Cycle complete. Outer door sealed.";
        
        text.text = notifText;

        cursor.transform.SetAsLastSibling();
    }

    IEnumerator CursorBlink()
    {
        Image cursorImage = cursor.GetComponent<Image>();

        while (true)
        {
            Color c = cursorImage.color;
            c.a = c.a > 0f ? 0f : 1f;
            cursorImage.color = c;

            yield return new WaitForSeconds(0.5f);
        }
    }
}
