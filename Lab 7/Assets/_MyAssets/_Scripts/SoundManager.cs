using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    public enum SoundType
    {
        SOUND_SFX,
        SOUND_MUSIC
    }

    public static SoundManager Instance { get; private set; } // Static object of the class.

    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicDictionary = new Dictionary<string, AudioClip>();
    private AudioSource sfxSource;
    private AudioSource musicSource;
    private float volumeMaster = 1.0f;
    private float volumeSfx = 1.0f;
    private float volumeMusic = 0.25f;

    // Initialize the SoundManager. I just put this functionality here instead of in the static constructor.
    public void Initialize(GameObject go)
    {
        sfxSource = go.AddComponent<AudioSource>();
        sfxSource.volume = volumeSfx * volumeMaster;
        
        musicSource = go.AddComponent<AudioSource>();
        musicSource.volume = volumeMusic * volumeMaster;
        musicSource.loop = true;
    }

    // Add a sound to the dictionary.
    public void AddSound(string soundKey, AudioClip audioClip, SoundType soundType)
    {
        Dictionary<string, AudioClip> targetDictionary = GetDictionaryByType(soundType);

        if (!targetDictionary.ContainsKey(soundKey))
        {
            targetDictionary.Add(soundKey, audioClip);
        }
        else
        {
            Debug.LogWarning("Sound key " + soundKey + " already exists in the " + soundType + " dictionary.");
        }
    }

    // Play a sound by key interface.
    public void PlaySound(string soundKey)
    {
        Play(soundKey, SoundType.SOUND_SFX);
    }

    public void PlayLoopedSound(string soundKey)
    {
        if (sfxSource.isPlaying) return;
        if (sfxDictionary.ContainsKey(soundKey))
        {
            sfxSource.clip = sfxDictionary[soundKey];
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopLoopedSound()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
        sfxSource.loop = false;
    }

    // Play music by key interface.
    public void PlayMusic(string soundKey)
    {
        musicSource.Stop();
        musicSource.clip = musicDictionary[soundKey];
        musicSource.Play();
    }

    // Play utility.
    private void Play(string soundKey, SoundType soundType)
    {
        Dictionary<string, AudioClip> targetDictionary;
        AudioSource targetSource;

        SetTargetsByType(soundType, out targetDictionary, out targetSource);

        if (targetDictionary.ContainsKey(soundKey))
        {
            targetSource.PlayOneShot(targetDictionary[soundKey]);
        }
        else
        {
            Debug.LogWarning("Sound key " + soundKey + " not found in the " + soundType + " dictionary.");
        }
    }

    public void SetVolume(float volume, SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                volumeSfx = volume;
                sfxSource.volume = volumeSfx * volumeMaster;
                break;
            case SoundType.SOUND_MUSIC:
                volumeMusic = volume;
                musicSource.volume = volumeMusic * volumeMaster;
                break;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                break;
        }
    }

    public void SetMasterVolume(float volume)
    {
        volumeMaster = volume;
        sfxSource.volume = volumeSfx * volumeMaster;
        musicSource.volume = volumeMusic * volumeMaster;
    }

    private void SetTargetsByType(SoundType soundType, out Dictionary<string, AudioClip> targetDictionary, out AudioSource targetSource)
    {
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                targetDictionary = sfxDictionary;
                targetSource = sfxSource;
                break;
            case SoundType.SOUND_MUSIC:
                targetDictionary = musicDictionary;
                targetSource = musicSource;
                break;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                targetDictionary = null;
                targetSource = null;
                break;
        }
    }
    private Dictionary<string, AudioClip> GetDictionaryByType(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                return sfxDictionary;
            case SoundType.SOUND_MUSIC:
                return musicDictionary;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                return null;
        }
    }

    private AudioSource GetSourceByType(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.SOUND_SFX:
                return sfxSource;
            case SoundType.SOUND_MUSIC:
                return musicSource;
            default:
                Debug.LogError("Unknown sound type: " + soundType);
                return null;
        }
    }
}