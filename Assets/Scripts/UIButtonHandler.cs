using System;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonHandler : MonoBehaviour
{
    [SerializeField]
    private Button UIStartButton;
    [SerializeField]
    private Button UIShootButton;
    [SerializeField]
    private Button UIResetButton;
    [SerializeField]
    private Button UIScoreButton;
    [SerializeField]
    private Button UITimeButton;
    [SerializeField]
    private Button UIRestartButton;
    [SerializeField]
    private Button UIRestartMainButton;


    public static event Action OnUIStartButtonPressed;
    public static event Action OnUIShootButtonPressed;
    public static event Action OnUIResetButtonPressed;

    public static event Action OnUITimeButtonPressed;
    public static event Action OnUIScoreButtonPressed;
    public static event Action OnUIRestartButtonPressed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIStartButton.onClick.AddListener(OnStartButtonPressed);
        UIShootButton.onClick.AddListener(OnShootButtonPressed);
        UIResetButton.onClick.AddListener(OnResetButtonPressed);

        UIScoreButton.onClick.AddListener(OnScoreButtonPressed);
        UITimeButton.onClick.AddListener(OnTimeButtonPressed);
        UIRestartButton.onClick.AddListener(OnRestartButtonPressed);
        UIRestartMainButton.onClick.AddListener(OnRestartButtonPressed);




        UIShootButton.gameObject.SetActive(false);

    }

    private void OnRestartButtonPressed()
    {
        OnUIRestartButtonPressed?.Invoke();
        UIStartButton.gameObject.SetActive(true);
        UIShootButton.gameObject.SetActive(false);
    }

    private void OnTimeButtonPressed()
    {
       OnUITimeButtonPressed?.Invoke();
    }

    private void OnScoreButtonPressed()
    {
        OnUIScoreButtonPressed?.Invoke();
    }

    private void OnStartButtonPressed()
    {
        OnUIStartButtonPressed?.Invoke();
        UIStartButton.gameObject.SetActive(false);
        UIShootButton.gameObject.SetActive(true);
    }


    private void OnShootButtonPressed()
    {
        OnUIShootButtonPressed?.Invoke();
    }

    private void OnResetButtonPressed()
    {
        OnUIResetButtonPressed?.Invoke();
        UIStartButton.gameObject.SetActive(true);
        UIShootButton.gameObject.SetActive(false);

    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
