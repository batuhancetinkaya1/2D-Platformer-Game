using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class AudioClipData
{
    public string clipName;
    public AudioClip clip;
    public float volume = 1f;
    public bool loop = false;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Clips")]
    public List<AudioClipData> audioClips;

    private Dictionary<string, AudioClipData> m_clipDict;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        m_clipDict = new Dictionary<string, AudioClipData>();
        foreach (var clipData in audioClips)
        {
            if (!m_clipDict.ContainsKey(clipData.clipName))
                m_clipDict.Add(clipData.clipName, clipData);
        }
    }

    public void PlayMusic(string clipName, bool restart = false)
    {
        if (!m_clipDict.ContainsKey(clipName))
        {
            Debug.LogWarning("Audio clip not found: " + clipName);
            return;
        }

        AudioClipData data = m_clipDict[clipName];
        if (musicSource.isPlaying && musicSource.clip == data.clip && !restart)
            return;

        musicSource.clip = data.clip;
        musicSource.volume = data.volume;
        musicSource.loop = data.loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlaySFX(string clipName)
    {
        if (!m_clipDict.ContainsKey(clipName))
        {
            Debug.LogWarning("SFX clip not found: " + clipName);
            return;
        }

        AudioClipData data = m_clipDict[clipName];
        sfxSource.PlayOneShot(data.clip, data.volume);
    }

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
