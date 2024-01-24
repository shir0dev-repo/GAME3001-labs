using System.IO;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip _soundtrack;
    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    private float _volume = 0.3f;

    private void Awake()
    {
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.clip = _soundtrack;
        _musicSource.playOnAwake = true;
        _musicSource.loop = true;
        _musicSource.volume = _volume;

        _sfxSource = gameObject.AddComponent<AudioSource>();
        _sfxSource.playOnAwake = false;
        _sfxSource.loop = false;
        _sfxSource.volume = _volume;
    }

    private void Start()
    {
        if (!_musicSource.isPlaying)
            _musicSource.Play();
    }

    public void SetVolume(float value)
    {
        _volume = value;
        _musicSource.volume = _volume;
        _sfxSource.volume = _volume;
    }

    public void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }
}
