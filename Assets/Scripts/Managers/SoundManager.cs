using System;
using UnityEngine;
using Random = UnityEngine.Random;

	
namespace LunarAnomaly
{
	[RequireComponent(typeof(AudioSource))]
	public class SoundManager : MonoBehaviour
	{
		[SerializeField] SoundList[] soundList;

		public static SoundManager Instance { get; private set; }
		
		AudioSource audioSource;

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

			audioSource = GetComponent<AudioSource>();
		}

        void OnValidate()
        {
            string [] names = Enum.GetNames(typeof(SoundType));
			Array.Resize(ref soundList, names.Length);
			for (int i = 0; i < soundList.Length; i++)
			{
				soundList[i].name = names[i];
			}
        }

        public static void PlaySound(SoundType sound, float volume = 1f, bool soundVariation = true)
		{
			if (Instance == null || Instance.audioSource == null)
			{
				Debug.Log("SoundManager instance not ready");
				return;
			}

			AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
			
			if (clips == null || clips.Length == 0)
			{
				Debug.LogWarning($"No clips assigned for sound: {sound}");
				return;
			}

			AudioClip randomClip = clips[Random.Range(0, clips.Length)];

			Instance.audioSource.pitch = soundVariation ? Random.Range(0.85f, 1.15f) : 1f;

			Instance.audioSource.PlayOneShot(randomClip, volume);
		}
	}

	public enum SoundType
	{
		Airlock,
		Mining,
		Ambience,
		Music,
		Footstep,
		GainAtmosphere,
		LoseAtmosphere,
		Alarm,
		RockBreak,
		Pickup,
		OutpostBang,
		OutpostSqueak,
		SwitchFlip,
		MachineStart,
		LeverPull,
		AlienSeenFirstTime
	}

	[Serializable]
	public struct SoundList
	{
		public AudioClip[] Sounds { get => sounds; }
		[HideInInspector] public string name;
		[SerializeField] AudioClip[] sounds;
	}
}
