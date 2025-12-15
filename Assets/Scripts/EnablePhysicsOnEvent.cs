using System;
using UnityEngine;

public class EnablePhysicsOnEvent : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIButtonHandler.OnUIStartButtonPressed += StartPhysicsOnButtonPressed; 
        rb.isKinematic = true;
    }

    private void StartPhysicsOnButtonPressed()
    {
        rb.isKinematic = false;
        rb.useGravity = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        UIButtonHandler.OnUIStartButtonPressed -= StartPhysicsOnButtonPressed;
    }
}
