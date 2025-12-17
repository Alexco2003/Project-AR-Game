using System;
using UnityEngine;

public class HideScore : MonoBehaviour
{
    [SerializeField]
    private Canvas ScoreUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIButtonHandler.OnUIStartButtonPressed += ShowScoreUI;
        UIButtonHandler.OnUIResetButtonPressed += HideScoreUI;
        UIButtonHandler.OnUIRestartButtonPressed += HideScoreUI;
        HideScoreUI();
    }

    private void HideScoreUI()
    {
        ScoreUI.gameObject.SetActive(false);
    }

    private void ShowScoreUI()
    {
        ScoreUI.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIStartButtonPressed -= ShowScoreUI;
        UIButtonHandler.OnUIResetButtonPressed -= HideScoreUI;
        UIButtonHandler.OnUIRestartButtonPressed -= HideScoreUI;
    }
}
