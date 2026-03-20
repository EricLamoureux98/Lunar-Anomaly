using System;
using UnityEngine;

	
namespace LunarAnomaly
{
	[RequireComponent(typeof(AudioSource)), ExecuteInEditMode]
	public class SoundManager : MonoBehaviour
	{
		[SerializeField] SoundList[] soundList;

		public static SoundManager Instance { get; private set; }
		
		AudioSource audioSource;

		void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				if (Application.isPlaying)
					DontDestroyOnLoad(gameObject);
			}
			else if (Instance != this)
			{
				Destroy(gameObject);
			}

			audioSource = GetComponent<AudioSource>();
		}
#if UNITY_EDITOR
        void OnEnable()
        {
            string [] names = Enum.GetNames(typeof(SoundType));
			Array.Resize(ref soundList, names.Length);
			for (int i = 0; i < soundList.Length; i++)
			{
				soundList[i].name = names[i];
			}
        }
#endif

        public static void PlaySound(SoundType sound, float volume = 1f)
		{
			AudioClip[] clips = Instance.soundList[(int)sound].Sounds;
			
			if (clips == null || clips.Length == 0)
			{
				Debug.LogWarning($"No clips assigned for sound: {sound}");
				return;
			}

			AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
			Instance.audioSource.PlayOneShot(randomClip, volume);
		}
	}

	public enum SoundType
	{
		Airlock,
		Mining,
		Ambience,
		Music,
		Footstep
	}

	[Serializable]
	public struct SoundList
	{
		public AudioClip[] Sounds { get => sounds; }
		[HideInInspector] public string name;
		[SerializeField] AudioClip[] sounds;
	}
}
