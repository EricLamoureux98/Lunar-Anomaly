using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace LunarAnomaly.UI
{
	public class TerminalLogPanel : BasePanel
	{
		[Header("Terminal Logs")]
        [SerializeField] LogTextDatabase logTextDatabase;
        [SerializeField] TMP_Text logText;
        [SerializeField] Button logButtonPrefab;
        [SerializeField] Transform logButtonContainer;

        protected override void OnPanelShown()
        {
            terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
			terminalController.RequestCurrentMessage();
            CreateLogButtons();
        }

		void CreateLogButtons()
        {
            foreach (var log in logTextDatabase.logEntries)
            {
                Button button = Instantiate(logButtonPrefab, logButtonContainer);

                TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();

                // text.text = log
            }
        }
	}
}
