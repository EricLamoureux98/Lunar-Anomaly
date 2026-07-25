using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LunarAnomaly.UI
{
    // ********* THIS HAS BEEN DEPRECATED **********
	// public class TerminalLogPanel : BasePanel
	// {
	// 	[Header("Terminal Logs")]
    //     [SerializeField] LogTextDatabase logTextDatabase;
    //     //[SerializeField] TMP_Text logText;
    //     [SerializeField] Button logButtonPrefab;
    //     [SerializeField] Transform logButtonContainer;

    //     public static event Action<LogMessage> OnLogMessage;

    //     protected override void OnPanelShown()
    //     {
    //         terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
	// 		terminalController.RequestCurrentMessage();
    //         CreateLogButtons();
    //     }

	// 	void CreateLogButtons()
    //     {
    //         // Makes sure spawned buttons update with isDiscoverd
    //         foreach (Transform child in logButtonContainer)
    //             Destroy(child.gameObject);

    //         foreach (var log in logTextDatabase.logEntries)
    //         {
    //             Button button = Instantiate(logButtonPrefab, logButtonContainer);
    //             TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
    //             text.text = log.logTitle;

    //             //button.interactable = log.isDiscovered;
    //             button.gameObject.SetActive(log.isDiscovered);

    //             LogMessage capturedMessage = log.message;
    //             button.onClick.AddListener(() => OnLogButtonClicked(capturedMessage));
    //         }

    //         void OnLogButtonClicked(LogMessage message)
    //         {
    //             // safe to remove later
    //             //logText.text = logTextDatabase.GetLogText(message);

    //             OnLogMessage?.Invoke(message);
    //         }
    //     }
	// }
}
