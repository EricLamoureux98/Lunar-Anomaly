using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using TMPro;
using UnityEngine;
	
namespace LunarAnomaly.UI
	{
	public class TerminalUI : MonoBehaviour
	{
		[Header("Terminal UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;
        [SerializeField] GameObject terminalUIPanel;
		[SerializeField] GameObject terminalOpenText;

		void OnEnable()
        {
            TerminalController.OnTerminalProximity += TerminalInteract;
            PlayerState.OnTerminalUIActive += TerminalPanel;
            TerminalController.OnTerminalDeposit += UpdateSamplesDeposited;
        }

        void OnDisable()
        {
            TerminalController.OnTerminalProximity -= TerminalInteract;
            PlayerState.OnTerminalUIActive -= TerminalPanel;
            TerminalController.OnTerminalDeposit -= UpdateSamplesDeposited;
        }

        void UpdateSamplesDeposited(int samples, int required)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples deposited: {0}/{1}", Mathf.Min(samples, required), required);
        }

		void TerminalInteract(bool value)
        {
            terminalOpenText.SetActive(value);
        }
        
        void TerminalPanel(bool value)
        {
            terminalUIPanel.SetActive(value);
        }
	}
}
