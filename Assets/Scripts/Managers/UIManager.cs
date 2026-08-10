using UnityEngine;
using System;
using UnityEngine.InputSystem;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Input;
using LunarAnomaly.Player;

namespace LunarAnomaly.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup gameplayCanvasGroup;

        [SerializeField] NotificationController notifController;
        [SerializeField] ScreenEffect screenEffect;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] PlayerState playerState;
        [SerializeField] TerminalUI terminalUI;
        [SerializeField] PauseMenu pauseMenu;

        bool terminalProximity;   

        // To PlayerLook
        public static event Action<bool> OnCursorUnlock;
        // To TerminalUI
        public static event Action<bool> OnDisplayTerminal;

        void OnEnable()
        {
            //PlayerState.OnHideGameplayUI += HideGameplayUI;
            GameManager.OnGameStateChanged += HandleGameStateChange;
            TerminalController.OnTerminalProximity += TerminalProximity;
            InputHandler.OnInteractPressed += TerminalInteract;
            InputHandler.OnCloseUI += DetermineESCInput;
        }

        void OnDisable()
        {
            //PlayerState.OnHideGameplayUI -= HideGameplayUI;
            GameManager.OnGameStateChanged -= HandleGameStateChange;
            TerminalController.OnTerminalProximity -= TerminalProximity;
            InputHandler.OnInteractPressed -= TerminalInteract;
            InputHandler.OnCloseUI -= DetermineESCInput;
        }

        void Update()
        {
            //Debug.Log(playerInput.currentActionMap.name);
        }

        void DetermineESCInput()
        {
            if (terminalUI.TerminalActive)
            {
                TryExitTerminal();         
            } 
            else if (GameManager.Instance.CurrentState == GameState.Playing)            
            {
                GameManager.Instance.ChangeState(GameState.Paused);         
            } 
            else if (GameManager.Instance.CurrentState == GameState.Paused)
            {
                // HandleGameStateChange(GameState.Paused);
                GameManager.Instance.ChangeState(GameState.Playing);

                //pauseMenuOpen = true;
                // gm.TogglePause();
                // TerminalUI.OnPanelSelected?.Invoke(PanelType.Pause);
            }
        }

        void HandleGameStateChange(GameState newState)
        {
            switch (newState)
            {
                case GameState.MainMenu:
                    
                    break;

                case GameState.Playing:
                    OnCursorUnlock?.Invoke(false);
                    TerminalUI.OnPanelClosed?.Invoke(); 
                    HideGameplayUI(false);
                    break;
                
                case GameState.GameOver:
                    HideGameplayUI(true);
                    screenEffect.GameOver();
                    break;
                
                case GameState.Paused:
                    OnCursorUnlock?.Invoke(true);
                    HideGameplayUI(true);
                    TerminalUI.OnPanelSelected?.Invoke(PanelType.Pause);
                    break;
            }
        }

        void HideGameplayUI(bool hidden)
        {
            if (hidden)
                gameplayCanvasGroup.alpha = 0f;
            else
                gameplayCanvasGroup.alpha = 1f;
        }

        void TerminalProximity(bool proximity)
        {
            terminalProximity = proximity;
        }
        
        void TerminalInteract()
        {
            if (playerState.CurrentState != PlayerCurrentState.Alive) return;

            TryEnterTerminal();
        }
        
        void TryEnterTerminal()
        {
            if (!terminalProximity) return;
            if (notifController.WaitingForReveal) return;

            EnterTerminal();
        }

        void TryExitTerminal()
        {
            if (terminalUI.TerminalActive)
            {
                ExitTerminal();
            }
        }

        void EnterTerminal()
        {
            //playerInput.SwitchCurrentActionMap("UI");
            OnCursorUnlock?.Invoke(true);
            OnDisplayTerminal?.Invoke(true);
            HideGameplayUI(true);
        }

        void ExitTerminal()
        {
            //playerInput.SwitchCurrentActionMap("Gameplay");
            OnCursorUnlock?.Invoke(false);
            OnDisplayTerminal?.Invoke(false);
            HideGameplayUI(false);
        }
    }
}