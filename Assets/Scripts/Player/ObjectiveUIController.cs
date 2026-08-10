using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LunarAnomaly.Gameplay;

namespace LunarAnomaly.UI
{
    public class ObjectiveUIController : MonoBehaviour
    {
        [SerializeField] TMP_Text objectiveTitle;
        [SerializeField] TMP_Text objectiveProgress;
        [SerializeField] Slider objectiveProgressBar;

        void OnEnable()
        {
            ObjectiveManager.OnUpdateObjectiveTitle += UpdateTitle;
            ObjectiveManager.OnUpdateObjectiveData += UpdateObjectiveInfo;
        }

        void OnDisable()
        {
            ObjectiveManager.OnUpdateObjectiveTitle -= UpdateTitle;
            ObjectiveManager.OnUpdateObjectiveData -= UpdateObjectiveInfo;
        }

        void UpdateTitle(string title)
        {
            objectiveTitle.text = title;
        }

        void UpdateObjectiveInfo(ObjectiveUIData data)
        {
            switch (data.Type)
            {
                case ObjectiveType.Full:
                    if (data.Current == null || data.Required == null) return;

                    objectiveProgress.text = string.Format("{0} / {1} {2}", data.Current, data.Required, data.TypeText);
                    objectiveProgressBar.gameObject.SetActive(true);
                    UpdateProgressBar(data.Current.Value, data.Required.Value);
                    break;

                case ObjectiveType.NoBar:
                    if (data.Current == null || data.Required == null) return;
                    objectiveProgressBar.gameObject.SetActive(false);
                    objectiveProgress.text = string.Format("{0} / {1} {2}", data.Current, data.Required, data.TypeText);
                    break; 

                case ObjectiveType.NoProgress:
                    objectiveProgress.text = "";
                    objectiveProgressBar.gameObject.SetActive(false);
                    break;
            }
        }

        void UpdateProgressBar(int progress, int remaining)
        {
            if (remaining <= 0) return;

            progress = Mathf.Clamp(progress, 0, remaining);

            float normalized = (float)progress / remaining;

            objectiveProgressBar.value = normalized;
        }
    }

    public enum ObjectiveType
    {
        Full,
        NoBar,
        NoProgress
    }
}

