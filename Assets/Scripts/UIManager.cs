using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LunarAnomaly.Gameplay;
using LunarAnomaly.Player;

namespace LunarAnomaly.UI
{
    // Disable overlay UI when in terminal
    public class UIManager : MonoBehaviour
    {
        [SerializeField] ScreenFader screenFader;

        [Header("Mining UI")]
        [SerializeField] TextMeshProUGUI samplesCollectedText;

        void OnEnable()
        {
            PlayerState.OnPlayerDying += PlayerDying;
            MiningManager.OnSamplesCarriedChanged += UpdateMiningSampleUI;
        }

        void OnDisable()
        {
            PlayerState.OnPlayerDying -= PlayerDying;
            MiningManager.OnSamplesCarriedChanged -= UpdateMiningSampleUI;
        }

        void UpdateMiningSampleUI(int samples)
        {
            //samplesCollectedText.text = string.Format("Samples collected: {0}/{1}", samples, remaining);
            samplesCollectedText.text = string.Format("Samples collected: {0}", samples);
        }

        void PlayerDying(float fadeLength)
        {
            StartCoroutine(DeathSequence(fadeLength));
        }

        IEnumerator DeathSequence(float duration)
        {
            screenFader.StartFade(0f, 1f, duration);
            yield return new WaitForSeconds(4f); // Adjust this for respawn timing
            screenFader.StartFade(1f, 0f, duration / 2);
        }
    }
}
