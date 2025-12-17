using System;
using UnityEngine;

public class HideEnd : MonoBehaviour
{
    [SerializeField]
    private Canvas EndUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        EndUI.gameObject.SetActive(false);
        UIButtonHandler.OnUIRestartButtonPressed += HideEndUI;

    }

    private void HideEndUI()
    {
        EndUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIRestartButtonPressed -= HideEndUI;
    }
}
