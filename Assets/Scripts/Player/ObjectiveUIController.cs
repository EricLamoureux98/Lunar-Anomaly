using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        //void UpdateObjectiveInfo(ObjectiveType type, int? progress = null, int? remaining = null, string typeTxt = "")
        void UpdateObjectiveInfo(ObjectiveUIData data)
        {
            switch (data.Type)
            {
                case ObjectiveType.Full:
                    if (data.Progress == null || data.Remaining == null) return;

                    objectiveProgress.text = string.Format("{0} / {1} {2}", data.Progress, data.Remaining, data.TypeText);
                    objectiveProgressBar.gameObject.SetActive(true);
                    UpdateProgressBar(data.Progress.Value, data.Remaining.Value);
                    break;

                case ObjectiveType.NoBar:
                    if (data.Progress == null || data.Remaining == null) return;
                    objectiveProgressBar.gameObject.SetActive(false);
                    objectiveProgress.text = string.Format("{0} / {1} {2}", data.Progress, data.Remaining, data.TypeText);
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

