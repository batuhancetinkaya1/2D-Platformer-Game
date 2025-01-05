using UnityEngine;
using System.Collections;
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

    // Playlist coroutine referansý
    private Coroutine m_playlistRoutine;

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

    #region Tekil Müzik Çalma
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
        musicSource.loop = data.loop; // Eðer tek parça loop edecekse burasý true
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    #endregion

    #region Playlist Sistemi
    /// <summary>
    /// Verilen klip isimlerini sýrayla çalar, en son klip bitince baþa döner.
    /// </summary>
    public void PlaySequentialMusic(params string[] clipNames)
    {
        // Eðer daha önce bir playlist çalýnýyorsa durdur.
        if (m_playlistRoutine != null)
        {
            StopCoroutine(m_playlistRoutine);
            m_playlistRoutine = null;
        }

        // Yeni playlist coroutine'i baþlat
        m_playlistRoutine = StartCoroutine(PlaySequentialRoutine(clipNames));
    }

    /// <summary>
    /// Playlist'i durdurur (ve eðer çalýyorsa müzikSource da durdurur).
    /// </summary>
    public void StopPlaylist()
    {
        if (m_playlistRoutine != null)
        {
            StopCoroutine(m_playlistRoutine);
            m_playlistRoutine = null;
        }
        StopMusic();
    }

    private IEnumerator PlaySequentialRoutine(string[] clipNames)
    {
        while (true) // Sürekli döngü
        {
            foreach (string clipName in clipNames)
            {
                if (!m_clipDict.ContainsKey(clipName))
                {
                    Debug.LogWarning("Audio clip not found in playlist: " + clipName);
                    continue;
                }

                AudioClipData data = m_clipDict[clipName];
                musicSource.loop = false; // Playlist mantýðý için tek tek parçalarý loop kapatarak çalýyoruz
                musicSource.clip = data.clip;
                musicSource.volume = data.volume;
                musicSource.Play();

                // Müzik bitene kadar bekleyelim
                yield return new WaitWhile(() => musicSource.isPlaying);

                // Bir sonraki þarkýya geçecek
            }
            // Tüm liste bitince tekrar baþa döner (while(true) sayesinde)
        }
    }
    #endregion

    #region SFX Çalma
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

    public void PlaySFXWithNewSource(string clipName, Vector3 position)
    {
        if (!m_clipDict.ContainsKey(clipName))
        {
            Debug.LogWarning("SFX clip not found: " + clipName);
            return;
        }

        AudioClipData data = m_clipDict[clipName];

        AudioSource tempSource = new GameObject("TempAudio").AddComponent<AudioSource>();
        tempSource.transform.position = position;
        tempSource.clip = data.clip;
        tempSource.volume = data.volume;
        tempSource.loop = data.loop;
        tempSource.Play();

        Destroy(tempSource.gameObject, data.clip.length);
    }
    #endregion

    #region Volume Kontrol
    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
            Debug.Log($"Music volume set to {volume}");
        }
        else
        {
            Debug.LogWarning("MusicSource is null.");
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
            Debug.Log($"SFX volume set to {volume}");
        }
        else
        {
            Debug.LogWarning("SFXSource is null.");
        }
    }
    #endregion
}
