using UnityEngine;
using PrimeTween;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class DataScores : IEquatable<DataScores>, IComparable<DataScores>
{
    public float score;
    public string name;
    public string date;

    public int CompareTo(DataScores other)
    {
        if (other == null)
            return 1;
        else
            return other.score.CompareTo(score);
    }

    public bool Equals(DataScores other)
    {
        if (other == null) 
            return false;
        return Mathf.Approximately(score, other.score);
    }
}


public class UIManager : MonoBehaviour
{
    [Header("Canvas Group")]
    public CanvasGroup menuHolder;
    public CanvasGroup gameHolder;

    [Header("Manager")]
    public GameManager gameManager;

    [Header("Other")]
    public GameEvents gameEvents = null;
    Sequence s;
    float bestScore; // Visual baseline for the current animation
    float persistentBestScore; // Absolute high score (saved immediately)
    float currentTargetScore;
    List<DataScores> dataScores;

    [Header("UI Elements")]
    public RectTransform logo;
    public Button button_play;
    public Button button_info;
    public Button button_user;
    [Space]
    public TextMeshProUGUI ratingHolderHeading;
    [Space]
    public Button button_setSores;
    public Text textField_PlayerName;
    [Space]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI roundText;
    [Header("Lose Popup")]
    public GameObject losePopup;
    public TextMeshProUGUI popupScoreText;
    public Button playAgainButton;
    [Space]
    public GameObject ratingTable;

    [Header("UI Params")]
    public float durationShake;
    public Vector3 strengthShake1;
    public Vector3 strengthShake3;
    public Vector3 strengthShake2;
    public Vector3 strengthShakeLogo;

    [Header("Audio Sliders")]
    public Slider bgMusicSlider;
    public Slider gameStartSlider;
    public Slider gameOverSlider;
    public Slider moveDownSlider;
    public Slider popSlider;
    public Slider attachSlider;
    public Slider throwBallSlider;
    public Slider scoreSlider;
    public Slider dropSlider;

    void ShowHolder(Action hideHolder, Action showHolder)
    {
        if (showHolder == null || hideHolder == null)
            return;

        hideHolder();
        
            showHolder();
    }

    Queue<float> scoreUpdateQueue = new Queue<float>();
    bool isUpdatingScore = false;

    IEnumerator CalculateScore(float scoreToAdd)
    {
        isUpdatingScore = true;
        float targetScore = gameEvents.score + scoreToAdd;
        float currentDisplayScore = gameEvents.score;

        while (currentDisplayScore < targetScore)
        {
            // Calculate a speed that finishes in a reasonable time but feels smooth
            float step = Mathf.Max(1f, (targetScore - currentDisplayScore) / 10f);
            currentDisplayScore += step;
            
            if (currentDisplayScore > targetScore) currentDisplayScore = targetScore;
            
            gameEvents.score = (int)currentDisplayScore;
            if (scoreText != null) scoreText.text = "Score\n" + gameEvents.score.ToString();
            
            // This now handles simultaneous tick-up if current score exceeds original best
            UpdateBestScoreUI();
            
            yield return null;
        }

        // Bake in the new visual baseline after animation finishes
        bestScore = Mathf.Max(gameEvents.score, bestScore);
        
        isUpdatingScore = false;
        if (scoreUpdateQueue.Count > 0)
        {
            StartCoroutine(CalculateScore(scoreUpdateQueue.Dequeue()));
        }
    }

    void CheckBestScore(float scoreToCheck)
    {
        if (scoreToCheck > persistentBestScore)
        {
            persistentBestScore = scoreToCheck;
            PlayerPrefs.SetFloat("BestScore", persistentBestScore);
            PlayerPrefs.Save();
            
            // If we are NOT animating, we can keep visual best in sync immediately
            if (!isUpdatingScore)
            {
                bestScore = persistentBestScore;
                UpdateBestScoreUI();
            }
        }
    }

    void UpdateBestScoreUI()
    {
        if (bestScoreText != null)
        {
            // Show the higher of current animated score or the original high score
            float visualBest = Mathf.Max(gameEvents.score, bestScore);
            bestScoreText.text = "Best\n" + ((int)visualBest).ToString();
        }
    }

    void ScoreUpdate(float scoreToAdd)
    {
        if (scoreToAdd == 0)
        {
            gameEvents.score = 0;
            currentTargetScore = 0;
            bestScore = persistentBestScore; 
            if (scoreText != null) scoreText.text = "Score\n0";
            scoreUpdateQueue.Clear();
            UpdateBestScoreUI();
        }
        else
        {
            currentTargetScore += scoreToAdd;
            CheckBestScore(currentTargetScore); // Save immediately!

            if (isUpdatingScore)
            {
                scoreUpdateQueue.Enqueue(scoreToAdd);
            }
            else
            {
                StartCoroutine(CalculateScore(scoreToAdd));
            }
        }
    }

    void RefreshUIElements()
    {
        if (roundText != null) roundText.text = "Round\n" + gameEvents.round.ToString();
        if (scoreText != null) scoreText.text = "Score\n" + ((int)gameEvents.score).ToString();
        currentTargetScore = gameEvents.score;
        persistentBestScore = PlayerPrefs.GetFloat("BestScore", 0);
        bestScore = Mathf.Max(gameEvents.score, persistentBestScore);
        CheckBestScore(currentTargetScore);
        UpdateBestScoreUI();
    }

    private void OnEnable()
    {
        gameEvents.RefreshUIElements += RefreshUIElements;
        gameEvents.ScoreUpdated += ScoreUpdate;
        gameEvents.SetHighSores += SetHighSores;
    }

    private void OnDisable()
    {
        gameEvents.RefreshUIElements -= RefreshUIElements;
        gameEvents.ScoreUpdated -= ScoreUpdate;
        gameEvents.SetHighSores -= SetHighSores;
    }

    Color RandomColor()
    {
        return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1);
    }

    void ButtonInitialize()
    {
        button_play.onClick.AddListener(() => {
            ShowHolder(
               () => {
                   s.Stop();
                   menuHolder.blocksRaycasts = false;
                   menuHolder.alpha = 0f;
               },
               () => {
                   gameHolder.blocksRaycasts = true;
                   gameManager.enabled = true;
                   gameHolder.alpha = 1f;
               }
           );
        });

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(PlayAgain);
        }
    }

    void PlayAgain()
    {
        Debug.Log("PlayAgain button clicked");
        if (losePopup != null) losePopup.SetActive(false);
        
        AudioManager.Instance.ResumeMusic();

        // Reset game events and scoring
        gameEvents.score = 0;
        currentTargetScore = 0;
        gameEvents.round = 1;
        gameEvents.ScoreUpdated?.Invoke(0);
        gameEvents.RefreshUIElements?.Invoke();
        
        // Cycle the GameManager to trigger OnEnable/Restart logic
        gameManager.enabled = false;
        gameManager.enabled = true;

        // Ensure UI state
        gameHolder.blocksRaycasts = true;
        gameHolder.alpha = 1f;
    }

    void InitializeAudioSliders()
    {
        if (AudioManager.Instance == null) return;

        if (bgMusicSlider != null) bgMusicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        if (gameStartSlider != null) gameStartSlider.onValueChanged.AddListener(AudioManager.Instance.SetGameStartVolume);
        if (gameOverSlider != null) gameOverSlider.onValueChanged.AddListener(AudioManager.Instance.SetGameOverVolume);
        if (moveDownSlider != null) moveDownSlider.onValueChanged.AddListener(AudioManager.Instance.SetMoveDownVolume);
        if (popSlider != null) popSlider.onValueChanged.AddListener(AudioManager.Instance.SetPopVolume);
        if (attachSlider != null) attachSlider.onValueChanged.AddListener(AudioManager.Instance.SetAttachVolume);
        if (throwBallSlider != null) throwBallSlider.onValueChanged.AddListener(AudioManager.Instance.SetThrowVolume);
        if (scoreSlider != null) scoreSlider.onValueChanged.AddListener(AudioManager.Instance.SetScoreVolume);
        if (dropSlider != null) dropSlider.onValueChanged.AddListener(AudioManager.Instance.SetDropVolume);
    }


    void SetHighSores()
    {
        // Don't disable raycasts on the holder if the popup is a child
        gameManager.enabled = false;
        
        // Show rating text if available
        if (ratingHolderHeading != null)
        {
            ratingHolderHeading.text = "GAME OVER";
        }

        // Show and animate the lose popup
        if (losePopup != null)
        {
            losePopup.SetActive(true);
            losePopup.transform.localScale = Vector3.zero;
            Tween.Scale(losePopup.transform, Vector3.one, 0.5f, Ease.OutBack);
            
            if (popupScoreText != null)
            {
                popupScoreText.text = ((int)currentTargetScore).ToString();
            }
        }
        
        AudioManager.Instance.StopMusic();
    }

    private void Start()
    {
        gameEvents.score = 0;
        currentTargetScore = 0;
        persistentBestScore = PlayerPrefs.GetFloat("BestScore", 0);
        bestScore = persistentBestScore;
        UpdateBestScoreUI();
       
        ButtonInitialize();
        InitializeAudioSliders();
    }
}
