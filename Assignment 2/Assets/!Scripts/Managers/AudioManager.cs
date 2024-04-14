using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    private AudioClip _musicTheme;
    private Dictionary<string, AudioClip> _audioDictionary = new();

    private AudioSource _musicSource, _sfxSource;

    public bool CurrentlyPlaying => _sfxSource.isPlaying;

    protected override void Awake()
    {
        base.Awake();

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.volume = 0.4f;
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.volume = 0.2f;
        _musicSource.playOnAwake = false;
        _musicSource.loop = true;

        _musicTheme = Resources.Load<AudioClip>("Audio/Forest Ambience");

        _audioDictionary.Add("Error", Resources.Load<AudioClip>("Audio/FFXIV_Error"));
        _audioDictionary.Add("ChangeCam", Resources.Load<AudioClip>("Audio/FFXIV_Camera_Mode"));
        _audioDictionary.Add("Select", Resources.Load<AudioClip>("Audio/FFXIV_Confirm"));
        _audioDictionary.Add("Complete", Resources.Load<AudioClip>("Audio/FFXIV_Log_out"));
        _audioDictionary.Add("Walk", Resources.Load<AudioClip>("Audio/Walk"));
    }

    private void Start()
    {
        _musicSource.clip = _musicTheme;
        _musicSource.Play();
    }

    public void PlaySoundEffect(string key)
    {
        PlaySoundEffect(key, 0.4f);
    }

    public void PlaySoundEffect(string key, float volume = 0.4f)
    {
        _sfxSource.PlayOneShot(_audioDictionary[key], volume);
    }
}
