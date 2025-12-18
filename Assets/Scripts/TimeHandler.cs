using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeHandler : MonoBehaviour
{
    [SerializeField] private Text timeText;

    [SerializeField] private bool startBuildOnAwake = true;

    // Build timing (measures how long player spent building before pressing START)
    private bool isBuilding;
    private float buildStartTime;
    private float buildElapsed;

    // Countdown timing (after pressing START). Countdown = buildDuration / 2
    private bool isCountingDown;
    private float countdownDuration;
    private float remainingTime;

    public static event Action OnCountdownFinished;

    public static event Action OnCountdownChanged;

    private void OnEnable()
    {
        UIButtonHandler.OnUIStartButtonPressed += HandleStartPressed;
        UIButtonHandler.OnUITimeButtonPressed += HandleTimeChanged;
        UIButtonHandler.OnUIResetButtonPressed += HandleResetPressed;
        ScoreHandler.OnScoreChanged += HandleScoreChanged;
        UIButtonHandler.OnUIRestartButtonPressed += HandleRestartPressed;

    }

    private void HandleRestartPressed()
    {
      
        isCountingDown = false;
        countdownDuration = 0f;
        remainingTime = 0f;

        isBuilding = true;
        buildElapsed = 0f;
        buildStartTime = Time.time;

        UpdateTimeTextDisplay(0f);
    }

    private void HandleResetPressed()
    {

        isCountingDown = false;
        isBuilding = true;
        buildStartTime = Time.time - buildElapsed;
        UpdateTimeTextDisplay(buildElapsed, true);
    }

    private void HandleTimeChanged()
    {
        var sh = FindFirstObjectByType<ScoreHandler>();
        if (sh == null || sh.GetScore() <= 0)
            return;

        OnCountdownChanged?.Invoke();
        remainingTime += 5f;
        UpdateTimeTextDisplay(remainingTime, false, true);
    }

    private void HandleScoreChanged()
    {
        remainingTime -= 1f;
        if (remainingTime < 0f)
        { remainingTime = 0f;
            isCountingDown = false;
            UpdateTimeTextDisplay(0f, false);
            OnCountdownFinished?.Invoke();
            return;
        }
        UpdateTimeTextDisplay(remainingTime, false, true);
    }

    private void OnDisable()
    {
        UIButtonHandler.OnUIStartButtonPressed -= HandleStartPressed;
        ScoreHandler.OnScoreChanged -= HandleScoreChanged;
        UIButtonHandler.OnUITimeButtonPressed -= HandleTimeChanged;
        UIButtonHandler.OnUIResetButtonPressed -= HandleResetPressed;
        UIButtonHandler.OnUIRestartButtonPressed -= HandleRestartPressed;
 
    }

    private void Awake()
    {
        if (startBuildOnAwake)
            StartBuildTimer();

        UpdateTimeTextDisplay(0f);
    }

    private void Start()
    {
        if (isBuilding)
            UpdateTimeTextDisplay(0f);
    }

    private void Update()
    {
        if (isBuilding)
        {
            buildElapsed = Time.time - buildStartTime;
            UpdateTimeTextDisplay(buildElapsed, true);
        }
        else if (isCountingDown)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0f)
            {
                remainingTime = 0f;
                isCountingDown = false;
                UpdateTimeTextDisplay(0f, false);
                OnCountdownFinished?.Invoke();
            }
            else
            {
                UpdateTimeTextDisplay(remainingTime, false, true);
            }
        }
    }
    public void StartBuildTimer()
    {
        isBuilding = true;
        isCountingDown = false;
        buildStartTime = Time.time;
        buildElapsed = 0f;
        UpdateTimeTextDisplay(0f, true);
    }

    private void HandleStartPressed()
    {

        if (isBuilding)
        {
            buildElapsed = Time.time - buildStartTime;
            float buildDuration = Mathf.Max(0f, buildElapsed);
            StartCountdown(buildDuration / 2f);
            isBuilding = false;
        }
        else
        {
            if (countdownDuration > 0f)
            {
                StartCountdown(countdownDuration);
            }
        }
    }

    public void StartCountdown(float seconds)
    {
        countdownDuration = Mathf.Max(0f, seconds);
        remainingTime = countdownDuration;
        isCountingDown = countdownDuration > 0f;
        isBuilding = false;

        UpdateTimeTextDisplay(remainingTime, false, true);

        if (countdownDuration == 0f)
        {
            isCountingDown = false;
            OnCountdownFinished?.Invoke();
        }
    }


    private void UpdateTimeTextDisplay(float valueSeconds, bool isElapsed = false, bool forceShowAsCountdown = false)
    {
 
        int totalSeconds;
        if (!isElapsed || forceShowAsCountdown)
            totalSeconds = Mathf.Max(0, Mathf.CeilToInt(valueSeconds));
        else
            totalSeconds = Mathf.Max(0, Mathf.FloorToInt(valueSeconds));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        string text = $"{minutes:00}:{seconds:00}";

        if (timeText != null)
            timeText.text = text;
    }

    public bool IsBuilding => isBuilding;
    public bool IsCountingDown => isCountingDown;
    public float GetBuildElapsedSeconds() => buildElapsed;
    public float GetRemainingSeconds() => remainingTime;
    public float GetCountdownDuration() => countdownDuration;
}