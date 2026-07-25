using System;
using LunarAnomaly.UI;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class ProgressionManager : MonoBehaviour
	{
		public ProgressionStage CurrentStage { get; private set; }

		// To ObjectiveManager
		public static event Action<ProgressionStage> OnStageChanged;
		// To TerminalInterfacePanel
		public static event Action<bool> OnInterfaceLock;

        void OnEnable()
        {
            TerminalInterfacePanel.OnPlayerProgressed += HandleConfirmed;
			OutpostRevealCinematic.OnOutpostMissionComplete += HandleConfirmed;
        }

        void OnDisable()
        {
            TerminalInterfacePanel.OnPlayerProgressed -= HandleConfirmed;
			OutpostRevealCinematic.OnOutpostMissionComplete -= HandleConfirmed;
        }

        void Start()
        {
            AdvanceStage(ProgressionStage.Intro);

			if (CurrentStage == ProgressionStage.Intro) 
				OnInterfaceLock?.Invoke(true);
        }

		void HandleConfirmed()
		{
			AdvanceStage(GetNextStage(CurrentStage));
		}

		ProgressionStage GetNextStage(ProgressionStage stage)
		{
			switch (stage)
			{
				case ProgressionStage.Intro:
					return ProgressionStage.OutpostObjective; // New stage

				case ProgressionStage.OutpostObjective:
					return ProgressionStage.SampleObjective;

				case ProgressionStage.SampleObjective:
					return ProgressionStage.Outro;
				
				case ProgressionStage.Outro:
					return stage;

				default:
					return stage;
			}
		}

		void AdvanceStage(ProgressionStage newStage)
		{
			if (newStage == CurrentStage) return;

			CurrentStage = newStage;
			OnStageChanged?.Invoke(CurrentStage);

			if (newStage != ProgressionStage.NoObjective)
				OnInterfaceLock?.Invoke(false);
		}
    }
}

public enum ProgressionStage
{
	Intro,
	SampleObjective,
	OutpostObjective,
	Outro,
	NoObjective
}
