using System;
using UnityEngine;

public class ResetRbPositionOfObject : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    private Vector3 rbStartPosition;
    private Quaternion rbStartRotation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIButtonHandler.OnUIResetButtonPressed += ResetRbPositionOnButtonPressed;

        rbStartPosition = rb.transform.localPosition;
        rbStartRotation = rb.transform.localRotation;

    }


    private void ResetRbPositionOnButtonPressed()
    {
        rb.isKinematic = true;

        rb.transform.localPosition = rbStartPosition;
        rb.transform.localRotation = rbStartRotation;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIResetButtonPressed -= ResetRbPositionOnButtonPressed;
    }
}
