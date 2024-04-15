using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    private AudioClip m_theme;

    private Dictionary<string, AudioClip> m_soundDictionary = new();
    private Dictionary<string, AudioClip> m_musicDictionary = new();
    private AudioSource m_sfxSource, m_musicSource;
    protected override void Awake()
    {
        base.Awake();

        m_sfxSource = gameObject.AddComponent<AudioSource>();
        m_musicSource = gameObject.AddComponent<AudioSource>();

        m_soundDictionary.Add("Win", Resources.Load<AudioClip>("Fanfare"));
        m_soundDictionary.Add("Woo", Resources.Load<AudioClip>("Woo"));
        m_soundDictionary.Add("Aww", Resources.Load<AudioClip>("Aww"));
        m_soundDictionary.Add("Lose", Resources.Load<AudioClip>("Lose"));
        m_musicDictionary.Add("Intro", Resources.Load<AudioClip>("Intro"));
        m_musicDictionary.Add("InGame", Resources.Load<AudioClip>("InGame"));

        m_musicSource.volume = 0.01f;
        m_musicSource.clip = m_theme;
        PlayTrack("Intro");
    }

    public void PlaySound(string key)
    {
        m_sfxSource.PlayOneShot(m_soundDictionary[key]);
    }

    public void PlayTrack(string key)
    {
        m_musicSource.clip = m_musicDictionary[key];
        m_musicSource.Play();
    }
}
