using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Settings")]
    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip menuMusic;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        musicSource = gameObject.AddComponent<AudioSource>();
        sfxSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = true;
        sfxSource.loop = false;

        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;

        menuMusic = Resources.Load<AudioClip>("Audio/menuLoop");

        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayMusic()
    {
        musicSource.Play();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audio/sfx/" + clipName);
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

/// <summary>
/// Sets the music clip to be played. The clip should be located in the Resources/Audio folder.
/// The clip name should not include the file extension (e.g., "menuLoop" instead of "menuLoop.mp3").
/// If a audio clip is curently playing, it will be stopped and replaced with the new one.
/// It will also directly play the new clip.
/// </summary>
/// <param name="clipName"> name of the audio clip </param>
/// <param name="stopCurrent"> if true, the current clip will be stopped before playing the new one </param>
/// <param name="playImmediately"> if true, the new clip will be played immediately </param>
    public void SetMusicClip(string clipName, bool stopCurrent = true, bool playImmediately = true)
    {
        if (stopCurrent && musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        AudioClip clip = Resources.Load<AudioClip>("Audio/" + clipName);
        if (clip != null)
        {
            musicSource.clip = clip;
            if (playImmediately)
            {
                musicSource.Play();
            }
        }
        else
        {
            Debug.LogError("Audio clip not found: " + clipName);
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }

}