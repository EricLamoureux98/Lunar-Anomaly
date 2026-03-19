using System;
using UnityEngine;

	
namespace LunarAnomaly
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance { get; private set; }
		public GameState CurrentState => gameState;

		GameState gameState;

		public static event Action<GameState> OnGameStateChanged;

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}

			ChangeState(GameState.Playing);
		}

        public void TriggerGameOver()
		{
			ChangeState(GameState.GameOver);
		}

		public void SetGamePaused()
		{
			ChangeState(GameState.Paused);
		}

		void ChangeState(GameState newState)
		{
			if (newState == gameState) return;

			if (gameState == GameState.GameOver && newState != GameState.MainMenu) return;

			gameState = newState;

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