using System;
using UnityEngine;

public class RewardFallDetector : MonoBehaviour
{
    [SerializeField] 
    private string groundTag = "Ground";

    public static event Action<GameObject> OnRewardLanded;

    private bool hasLanded;

    private void OnTriggerEnter(Collider other)
    {
        TryHandleLanding(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryHandleLanding(collision.gameObject);
    }

    private void TryHandleLanding(GameObject colliderObject)
    {
        if (hasLanded) return;

        if (!string.IsNullOrEmpty(groundTag))
        {
            if (!colliderObject.CompareTag(groundTag))
                return;
        }

        hasLanded = true;
        OnRewardLanded?.Invoke(gameObject);
    }
}