using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour 
{
    public AudioClip m_AmbientMusic;
    AudioSource m_MusicSource;

    public AudioClip[] m_Sounds;
    public int m_MaxSoundSources;
    List<AudioSource> m_SoundSources;

    float m_MasterMusicVolume = 0;
    float m_MusicVolume = 1;
    float m_SFXVolume = 1;

    void Start() 
    {
        SessionManager Session = GameObject.Find("SessionManager").GetComponent<SessionManager>();
        if (Session.m_AllowAudio)
        {
            InitSounds();
            InitMusic();

            PlayMusic();
        }
	}

    void InitMusic()
    {
        m_MusicSource = gameObject.AddComponent<AudioSource>();
        m_MusicSource.loop = true;
    }

    void PlayMusic()
    {
        m_MusicSource.clip = m_AmbientMusic;
        m_MusicSource.Play();
        m_MusicSource.volume = m_MasterMusicVolume * m_MusicVolume;
    }

    public void SetMusicVolume(float _Volume)
    {
        m_MusicVolume = _Volume;
        m_MusicSource.volume = m_MasterMusicVolume * m_MusicVolume;
    }

    void InitSounds()
    {
        // create some sources for later use
        m_SoundSources = new List<AudioSource>();
        for (int i = 0; i < m_MaxSoundSources; i++)
        {
            AudioSource NewSource = gameObject.AddComponent<AudioSource>();
            m_SoundSources.Add(NewSource);
        }
    }

    public void PlaySound(string _Name)
    {
        int Index = 0;
        for (; Index < m_Sounds.Length; Index++)
        {
            if (m_Sounds[Index].name == _Name)
                break;
        }

        if (Index != m_Sounds.Length)
        {
            // look for an unused source
            for (int i = 0; i < m_MaxSoundSources; i++)
            {
                if (!m_SoundSources[i].isPlaying)
                {
                    m_SoundSources[i].clip = m_Sounds[Index];
                    m_SoundSources[i].Play();
                    m_SoundSources[i].volume = m_SFXVolume;
                    break;
                }
            }
        }
    }

    public void UpdateSFX(int Enabled)
    {
        float volume = 0;
        if (Enabled == 1)
            volume = 1;
        m_SFXVolume = volume;
        if (m_SoundSources != null)
        {
            for (int i = 0; i < m_MaxSoundSources; i++)
            {
                m_SoundSources[i].volume = volume;
            }
        }
    }

    public void UpdateMusic(int Enabled)
    {
        if (Enabled == 0)
            m_MasterMusicVolume = 0;
        else
            m_MasterMusicVolume = 1;

        if (m_MusicSource != null)
            m_MusicSource.volume = m_MasterMusicVolume * m_MusicVolume;
    }
}
