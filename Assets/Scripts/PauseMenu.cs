using LunarAnomaly;
using LunarAnomaly.UI;
using UnityEngine;

public class PauseMenu : BasePanel
{    
    [SerializeField] CanvasGroup debugCanvas;

    bool debugEnabled;

    public void ReturnToMainMenu()
    {
        GameManager.Instance.ReturnToMainMenu();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }

    public void ResumeGame()
    {
        GameManager.Instance.TogglePause();
    }

    public void ToggleDebugOptions()
    {
        if (!debugEnabled)
        {
            debugEnabled = true;
            EnableDebugMenu(true);
        }
        else
        {
            debugEnabled = false;
            EnableDebugMenu(false);
        }
    }
    
    void EnableDebugMenu(bool enabled)
    {
        if (enabled)
            debugCanvas.alpha = 1f;
        else 
            debugCanvas.alpha = 0f;

		debugCanvas.interactable = enabled;
		debugCanvas.blocksRaycasts = enabled;
    }
}
