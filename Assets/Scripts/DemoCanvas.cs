using LunarAnomaly;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;
using LunarAnomaly.UI;
using TMPro;
using UnityEngine;

public class DemoCanvas : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TerminalUpdateText updateText;
    [SerializeField] TMP_Text currentTextBox;
	[SerializeField] Typewriter typewriter;
    // [SerializeField] PlayerLook playerLook;

    [Header("Logs")]
	[TextArea(5,10)]
	[SerializeField] string logText;

    [SerializeField] GameObject demoCanvas;

    GameManager gm;

    void OnEnable() => OutpostRevealCinematic.OnDemoComplete += ShowDemoCanvas;
    void OnDisable() => OutpostRevealCinematic.OnDemoComplete -= ShowDemoCanvas;

    void Awake()
    {
        gm = GameManager.Instance;
    }

    void ShowDemoCanvas()
    {
        demoCanvas.SetActive(true);
        //playerLook.UpdateCursorLock(true);
        OutpostUI.OnLogShown?.Invoke(true);

        updateText.UpdateCurrentTextBox(currentTextBox);
		updateText.ShowWithTypewriter(logText);
        //GameManager.Instance.TogglePause();
    }

    public void ReturnToMainMenu()
    {
        gm.ReturnToMainMenu();
    }
    
}
