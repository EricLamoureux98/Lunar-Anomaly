using System;
	
namespace LunarAnomaly.UI
{
	public class TerminalIntroPanel : BasePanel
	{		
        // To ProgressionManager
        public static event Action OnPlayerProgressed;

        protected override void OnPanelShown()
        {
            terminalUpdateText.UpdateCurrentTextBox(currentTextBox);
			terminalController.RequestCurrentMessage();
        }
		
		public void HandleIntroProceedButton()
        {
            if (terminalController.terminalActive)
            {
                //UpdateStage(ProgressionStage.SampleObjective);
                OnPlayerProgressed?.Invoke();
            }
        }	
	}
}
