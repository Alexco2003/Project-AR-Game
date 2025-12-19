using System;
using UnityEngine;
using UnityEngine.UI;

public class EndGameLogic : MonoBehaviour
{
    [SerializeField] 
    private Canvas startCanvas;
    [SerializeField] 
    private Canvas scoreCanvas;
    [SerializeField] 
    private Canvas endCanvas;

    [SerializeField] 
    private Text endGameText;

    [SerializeField] 
    private string[] targetTags = new string[] { "Crystal", "Ingot", "Jar" };


    [SerializeField] 
    private bool addRemainingTimeToScore = true;
    [SerializeField] 
    private int timeToScoreMultiplier = 5;

    private ScoreHandler scoreHandler;
    private TimeHandler timeHandler;

    private bool gameEnded;

    private void Awake()
    {
        scoreHandler = FindFirstObjectByType<ScoreHandler>();
        timeHandler = FindFirstObjectByType<TimeHandler>();
    }

    private void OnEnable()
    {
        TimeHandler.OnCountdownFinished += OnCountdownFinished;
        UIButtonHandler.OnUIRestartButtonPressed += OnRestartPressed;
    }

    private void OnDisable()
    {
        TimeHandler.OnCountdownFinished -= OnCountdownFinished;
        UIButtonHandler.OnUIRestartButtonPressed -= OnRestartPressed;
    }

    private void Start()
    {
        SetCanvasState(startVisible: true, scoreVisible: false, endVisible: false);
        gameEnded = false;
        if (endGameText != null) 
            endGameText.text = string.Empty;
    }

    private void Update()
    {
        if (gameEnded) 
            return;

        if (timeHandler != null && timeHandler.IsCountingDown)
        {
            if (AreAllTargetsCollected())
            {
                EndGame(win: true);
            }
        }
    }

    private bool AreAllTargetsCollected()
    {
        if (targetTags == null || targetTags.Length == 0) return false;

        for (int i = 0; i < targetTags.Length; i++)
        {
            var tag = targetTags[i];
            if (string.IsNullOrEmpty(tag)) continue;

            var found = GameObject.FindWithTag(tag);
            if (found != null)
                return false; 
        }

        return true;
    }

    private void OnCountdownFinished()
    {
        EndGame(win: true);
    }

    private void EndGame(bool win)
    {
        if (gameEnded) return;
            gameEnded = true;

        SetCanvasState(startVisible: false, scoreVisible: false, endVisible: true);

        int baseScore = scoreHandler != null ? scoreHandler.GetScore() : 0;
        int bonus = 0;
        float remaining = timeHandler != null ? timeHandler.GetRemainingSeconds() : 0f;

        if (addRemainingTimeToScore && remaining > 0f)
            bonus = Mathf.CeilToInt(remaining) * Math.Max(0, timeToScoreMultiplier);

        int finalScore = baseScore + bonus;
        string header = win ? "You won!" : "Game over";
        string scoreLine = $"Score: {finalScore}";
        string timeLine = remaining > 0f ? $"Time left: {Mathf.CeilToInt(remaining)}s" : string.Empty;
        if (endGameText != null)
        {
            if (string.IsNullOrEmpty(timeLine))
                endGameText.text = $"{header}\n{scoreLine}";
            else
                endGameText.text = $"{header}\n{scoreLine}\n{timeLine}";
        }
    }

    private void OnRestartPressed()
    {
        gameEnded = false;
        SetCanvasState(startVisible: true, scoreVisible: false, endVisible: false);

        if (endGameText != null) 
            endGameText.text = string.Empty;

    }

    private void SetCanvasState(bool startVisible, bool scoreVisible, bool endVisible)
    {
        if (startCanvas != null) 
            startCanvas.gameObject.SetActive(startVisible);
        if (scoreCanvas != null) 
            scoreCanvas.gameObject.SetActive(scoreVisible);
        if (endCanvas != null) 
            endCanvas.gameObject.SetActive(endVisible);
    }
}