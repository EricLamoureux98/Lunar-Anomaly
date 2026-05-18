using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LunarAnomaly
{
	public class GameManager : MonoBehaviour
	{
		[SerializeField] GameObject playerObject;
		
		public static GameManager Instance { get; private set; }

		public GameState CurrentState => gameState; 

		GameState gameState;

		public static event Action<GameState> OnGameStateChanged;

		void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(gameObject);
				return;
			}
			Instance = this;

			if (Application.isPlaying)
			{
				transform.SetParent(null); // Make it root to prevent warning
				DontDestroyOnLoad(gameObject);					
			}

			ChangeState(GameState.MainMenu);
		}

		public void RegisterPlayer(GameObject player)
		{
			playerObject = player;
		}

		public void PlayGame()
		{
			SceneManager.LoadScene(1);
			ChangeState(GameState.Playing);
		}

		public void QuitGame()
		{
			Application.Quit();

			#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
			#endif
		}

        public void TriggerGameOver()
		{
			ChangeState(GameState.GameOver);
		}

		// Add a button for this
		public void TogglePause()
		{
			if (gameState == GameState.Paused)
				ChangeState(GameState.Playing);
			else if (gameState == GameState.Playing)
				ChangeState(GameState.Paused);
		}

		public void ReturnToMainMenu()
		{
			SceneManager.LoadScene(0);
			ChangeState(GameState.MainMenu);
		}

		void ChangeState(GameState newState)
		{
			//if (playerObject == null) Debug.Log("Player not assigned in GM");

			if (newState == gameState) return;

			if (gameState == GameState.GameOver && newState != GameState.MainMenu) return;

			gameState = newState;

			switch(gameState)
			{
				case GameState.Playing:
					Time.timeScale = 1f;
					//playerObject.SetActive(true);
					break;

				case GameState.Paused:
					Time.timeScale = 0f;
					break;

				case GameState.MainMenu:
					break;
				
				default:
					Time.timeScale = 0f;
					//playerObject.SetActive(false);
					break;
			}

			OnGameStateChanged?.Invoke(gameState);
		}
	}

	public enum GameState
	{
		MainMenu,
		Playing,
		GameOver,
		Paused
	}
}