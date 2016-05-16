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
                    break;
                }
            }
        }
    }
}
