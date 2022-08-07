using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.Utils
{
	public class SoundsController : MonoBehaviour
	{
		private static SoundsController _instance;
		public static SoundsController Instance
		{
			get
			{
				if (!_instance)
				{
					_instance = GameObject.FindObjectOfType<SoundsController>();
				}
				return _instance;
			}
		}

		public AudioClip[] Sounds;
		public bool EnableSound = false;

		private AudioSource m_audioBackground;
		private AudioSource m_audioFX;

		void Awake()
		{
			AudioSource[] myAudioSources = GetComponents<AudioSource>();
			m_audioBackground = myAudioSources[0];
			m_audioFX = myAudioSources[1];
		}

		void Start()
		{
		}

		private void PlaySoundClipBackground(AudioClip _audio, bool _loop, float _volume)
		{
			if (!EnableSound) return;

			m_audioBackground.clip = _audio;
			m_audioBackground.loop = _loop;
			m_audioBackground.volume = _volume;
			m_audioBackground.Play();
		}

		public void StopSoundBackground()
		{
			m_audioBackground.clip = null;
			m_audioBackground.Stop();
		}

		public void PlaySoundBackground(string _audioName, bool _loop, float _volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == _audioName)
				{
					PlaySoundClipBackground(Sounds[i], _loop, _volume);
				}
			}
		}


		private void PlaySoundClipFx(AudioClip _audio, bool _loop, float _volume)
		{
			if (!EnableSound) return;

			m_audioFX.clip = _audio;
			m_audioFX.loop = _loop;
			m_audioFX.volume = _volume;
			m_audioFX.Play();
		}

		public void StopSoundFx()
		{
			m_audioFX.clip = null;
			m_audioFX.Stop();
		}

		public void PlaySoundFX(string _audioName, bool _loop, float _volume)
		{
			for (int i = 0; i < Sounds.Length; i++)
			{
				if (Sounds[i].name == _audioName)
				{
					PlaySoundClipFx(Sounds[i], _loop, _volume);
				}
			}
		}

	}
}