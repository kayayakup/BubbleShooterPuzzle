using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance = null;

    [Header("Background Music")]
    public AudioClip bgMusicClip;
    [Range(0, 1)] public float bgMusicVolume = 0.5f;

    [Header("Sound Effects")]
    public AudioClip gameStartClip;
    [Range(0, 1)] public float gameStartVolume = 1f;
    [Space]
    public AudioClip gameOverClip;
    [Range(0, 1)] public float gameOverVolume = 1f;
    [Space]
    public AudioClip moveDownClip;
    [Range(0, 1)] public float moveDownVolume = 1f;
    [Space]
    public AudioClip popClip;
    [Range(0, 1)] public float popVolume = 1f;
    [Space]
    public AudioClip attachClip;
    [Range(0, 1)] public float attachVolume = 1f;
    [Space]
    public AudioClip throwBallClip;
    [Range(0, 1)] public float throwBallVolume = 1f;
    [Space]
    public AudioClip scoreClip;
    [Range(0, 1)] public float scoreVolume = 1f;
    [Space]
    public AudioClip dropClip;
    [Range(0, 1)] public float dropVolume = 1f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            musicSource = gameObject.AddComponent<AudioSource>();
            sfxSource = gameObject.AddComponent<AudioSource>();
            
            InitializeMusic();
        }
    }

    private void InitializeMusic()
    {
        if (bgMusicClip != null)
        {
            musicSource.clip = bgMusicClip;
            musicSource.loop = true;
            musicSource.volume = bgMusicVolume;
            musicSource.Play();
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    public void ResumeMusic()
    {
        if (musicSource != null && !musicSource.isPlaying)
        {
            musicSource.Play();
        }
    }

    // Specific Play Methods
    public void PlayGameStart() => sfxSource.PlayOneShot(gameStartClip, gameStartVolume);
    public void PlayGameOver() => sfxSource.PlayOneShot(gameOverClip, gameOverVolume);
    public void PlayMoveDown() => sfxSource.PlayOneShot(moveDownClip, moveDownVolume);
    public void PlayPop() => sfxSource.PlayOneShot(popClip, popVolume);
    public void PlayAttach() => sfxSource.PlayOneShot(attachClip, attachVolume);
    public void PlayThrow() => sfxSource.PlayOneShot(throwBallClip, throwBallVolume);
    public void PlayScore() => sfxSource.PlayOneShot(scoreClip, scoreVolume);
    public void PlayDrop() => sfxSource.PlayOneShot(dropClip, dropVolume);

    // Volume Setters (Simplified for Sliders)
    public void SetMusicVolume(float val) { bgMusicVolume = val; musicSource.volume = val; }
    public void SetGameStartVolume(float val) => gameStartVolume = val;
    public void SetGameOverVolume(float val) => gameOverVolume = val;
    public void SetMoveDownVolume(float val) => moveDownVolume = val;
    public void SetPopVolume(float val) => popVolume = val;
    public void SetAttachVolume(float val) => attachVolume = val;
    public void SetThrowVolume(float val) => throwBallVolume = val;
    public void SetScoreVolume(float val) => scoreVolume = val;
    public void SetDropVolume(float val) => dropVolume = val;

    // Legacy support for string-based triggers if needed, but we'll update the scripts
    public void PlaySound(string soundName)
    {
        switch (soundName.ToLower())
        {
            case "gamestart": PlayGameStart(); break;
            case "gameover": PlayGameOver(); break;
            case "movedown": PlayMoveDown(); break;
            case "pop": PlayPop(); break;
            case "attach": PlayAttach(); break;
            case "throw": PlayThrow(); break;
            case "score": PlayScore(); break;
            case "drop": PlayDrop(); break;
        }
    }

    public void SetVolume(string soundName, float volume)
    {
        switch (soundName.ToLower())
        {
            case "bgmusic": SetMusicVolume(volume); break;
            case "gamestart": SetGameStartVolume(volume); break;
            case "gameover": SetGameOverVolume(volume); break;
            case "movedown": SetMoveDownVolume(volume); break;
            case "pop": SetPopVolume(volume); break;
            case "attach": SetAttachVolume(volume); break;
            case "throw": SetThrowVolume(volume); break;
            case "score": SetScoreVolume(volume); break;
            case "drop": SetDropVolume(volume); break;
        }
    }
}