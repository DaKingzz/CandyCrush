using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip buttonClick;
    [SerializeField] private AudioClip matchFail;
    [SerializeField] private AudioClip matchSuccess;
    [SerializeField] private AudioClip backgroundTheme;

    private static AudioManager instance;

    void Awake()
    {
        // Singleton pattern so it persists between scenes, which is what i want for theme song
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource && backgroundTheme)
        {
            musicSource.clip = backgroundTheme;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    // Static helpers for quick access
    public static void PlayButton() { instance?.PlaySFX(instance.buttonClick); }
    public static void PlayFail() { instance?.PlaySFX(instance.matchFail); }
    public static void PlaySuccess() { instance?.PlaySFX(instance.matchSuccess); }

    private void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }
}
