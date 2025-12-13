using UnityEngine;

public class GameMusicManager : MonoBehaviour
{
    public AudioSource musicSource;
    public static GameMusicManager instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.6f);
            SetVolume(savedVolume);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;

        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip = null, bool loop = true)
    {
        if (musicSource == null) return;

        if (clip != null) musicSource.clip = clip;

        musicSource.loop = loop;

        // Only play if not already playing
        if (!musicSource.isPlaying)
            musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();
    }

    // ✅ NEW: guarantees it will NOT keep playing into the restarted intro
    public void StopAndRewind()
    {
        if (musicSource == null) return;

        musicSource.Stop();
        musicSource.time = 0f;
    }
}
