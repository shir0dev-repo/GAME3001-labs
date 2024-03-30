using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [SerializeField] private AudioClip _musicTheme;
    private Dictionary<string, AudioClip> _audioDictionary = new();

    private AudioSource _musicSource, _sfxSource;

    protected override void Awake()
    {
        base.Awake();

        _sfxSource = gameObject.AddComponent<AudioSource>();

        _audioDictionary.Add("Error", Resources.Load<AudioClip>("Audio/FFXIV_Error"));
        _sfxSource.PlayOneShot(_audioDictionary["Error"]);
    }

}
