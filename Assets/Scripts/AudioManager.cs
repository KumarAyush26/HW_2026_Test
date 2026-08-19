using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip fallClip;
    [SerializeField] private AudioClip warningClip;
    [SerializeField] private AudioClip buttonClickClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (backgroundMusic != null && musicSource != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlayScore()
    {
        PlaySfx(scoreClip);
    }

    public void PlayFall()
    {
        PlaySfx(fallClip);
    }

    public void PlayWarning()
    {
        PlaySfx(warningClip);
    }

    public void PlayButtonClick()
    {
        PlaySfx(buttonClickClip);
    }

    private void PlaySfx(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.Stop();
        sfxSource.clip = clip;
        sfxSource.Play();
    }

    public void PauseMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
            musicSource.Play();
    }
}