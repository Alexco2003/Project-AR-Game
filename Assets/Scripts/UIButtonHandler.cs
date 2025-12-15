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


    public static event Action OnUIStartButtonPressed;
    public static event Action OnUIShootButtonPressed;
    public static event Action OnUIResetButtonPressed;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIStartButton.onClick.AddListener(OnStartButtonPressed);
        UIShootButton.onClick.AddListener(OnShootButtonPressed);
        UIResetButton.onClick.AddListener(OnResetButtonPressed);

        UIShootButton.gameObject.SetActive(false);

    }

    private void OnStartButtonPressed()
    {
        OnUIStartButtonPressed?.Invoke();
        UIStartButton.gameObject.SetActive(false);
        UIShootButton.gameObject.SetActive(true);
    }


    private void OnShootButtonPressed()
    {
        OnUIResetButtonPressed?.Invoke();
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
