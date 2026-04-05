using System;
using LunarAnomaly.UI;
using UnityEngine;

	
namespace LunarAnomaly.Gameplay
{
	public class ProgressionManager : MonoBehaviour
	{
		public ProgressionStage CurrentStage { get; private set; }

		public static event Action<ProgressionStage> OnStageChanged;

        void OnEnable()
        {
            TerminalIntroPanel.OnPlayerProgressed += HandleConfirmed;
        }

        void OnDisable()
        {
            TerminalIntroPanel.OnPlayerProgressed -= HandleConfirmed;
        }

        void Start()
        {
            AdvanceStage(ProgressionStage.Intro);
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
					return ProgressionStage.SampleObjective;

				case ProgressionStage.SampleObjective:
					return ProgressionStage.RepairObjective;

				case ProgressionStage.RepairObjective:
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
		}
    }
}

public enum ProgressionStage
{
	Intro,
	SampleObjective,
	RepairObjective,
	Outro
}
