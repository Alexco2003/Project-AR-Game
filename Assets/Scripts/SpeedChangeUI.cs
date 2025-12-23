using System;
using UnityEngine;
using UnityEngine.UI;

public class SpeedChangeUI : MonoBehaviour
{
    [SerializeField]
    private Text speedText;

    [SerializeField]
    private Slider speedSlider;

    public static event Action<int> OnSpeedChanged;

    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.AddListener(OnSliderValueChanged);
            UpdateSpeedText(Mathf.RoundToInt(speedSlider.value));
        }
    }

    private void OnDestroy()
    {
        if (speedSlider != null)
            speedSlider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        int intValue = Mathf.RoundToInt(value);
        UpdateSpeedText(intValue);
        OnSpeedChanged?.Invoke(intValue);
    }

    private void UpdateSpeedText(int speed)
    {
        if (speedText != null)
        {
            speedText.text = $"{speed}";
        }
    }
}