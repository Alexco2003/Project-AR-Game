using System;
using UnityEngine;

public class HideBar : MonoBehaviour
{
    [SerializeField]
    private Canvas ARMagicBar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIButtonHandler.OnUIStartButtonPressed += HideARMagicBarOnButtonPressed;
        UIButtonHandler.OnUIResetButtonPressed += ShowARMagicBarOnButtonPressed;
        UIButtonHandler.OnUIRestartButtonPressed += ShowARMagicBarOnButtonPressed;

    }

    private void ShowARMagicBarOnButtonPressed()
    {
        ARMagicBar.enabled = true;
    }

    private void HideARMagicBarOnButtonPressed()
    {
        ARMagicBar.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIStartButtonPressed -= HideARMagicBarOnButtonPressed;
        UIButtonHandler.OnUIResetButtonPressed -= ShowARMagicBarOnButtonPressed;
        UIButtonHandler.OnUIRestartButtonPressed -= ShowARMagicBarOnButtonPressed;
    }
}
