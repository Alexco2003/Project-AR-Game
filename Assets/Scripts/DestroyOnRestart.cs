using UnityEngine;

public class DestroyOnRestart : MonoBehaviour
{


    private void Start()
    {
           UIButtonHandler.OnUIRestartButtonPressed += OnRestartPressed;
    }

    private void OnRestartPressed()
    {



        Destroy(gameObject);


    }

    private void OnDestroy()
    {

        UIButtonHandler.OnUIRestartButtonPressed -= OnRestartPressed;
    }
}
