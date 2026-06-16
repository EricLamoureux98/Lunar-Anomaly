using LunarAnomaly.Gameplay;
using TMPro;
using UnityEngine;

	
namespace LunarAnomaly.UI
{
	public abstract class BasePanel : MonoBehaviour
	{
		[SerializeField] PanelType panelType;

		[Header("References")]
		[SerializeField] protected UpdateText terminalUpdateText;
		[SerializeField] protected TerminalController terminalController;
		[SerializeField] protected TMP_Text currentTextBox;
		
		CanvasGroup canvasGroup;

		protected virtual void OnEnable()
		{
			TerminalUI.OnPanelSelected += HandlePanelSelected;
			TerminalUI.OnTerminalClosed += HidePanel;
		}

		protected virtual void OnDisable()
		{
			TerminalUI.OnPanelSelected -= HandlePanelSelected;
			TerminalUI.OnTerminalClosed -= HidePanel;
		}

		protected virtual void Awake()
		{
			canvasGroup = GetComponent<CanvasGroup>();
			if (canvasGroup == null) Debug.Log("Pannel " + gameObject.name + "does not have canvas group");
		}

		void HandlePanelSelected(PanelType selectedPanel)
        {	
            if (selectedPanel == panelType)
            {
				ShowPanel();
				OnPanelShown();
            }
            else
            {
				HidePanel();
            }
        }

		protected void ShowPanel()
		{
			canvasGroup.alpha = 1f;
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}

		protected void HidePanel()
		{
			canvasGroup.alpha = 0f;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		protected virtual void OnPanelShown()
		{
			
		}
	}
}
