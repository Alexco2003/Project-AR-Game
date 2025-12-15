using System;
using Unity.XR.CoreUtils;
using UnityEngine;

public class ShootBallLogic : MonoBehaviour
{
    private Camera mainCam;
    private XROrigin xrOrigin;

    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private float ballForwardForce = 500f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xrOrigin = FindFirstObjectByType<XROrigin>();

        if (xrOrigin != null && xrOrigin.Camera != null)
        {
            mainCam = xrOrigin.Camera;
        }
        else
        {
            mainCam = FindObjectOfType<Camera>();
        }

        UIButtonHandler.OnUIShootButtonPressed += ShootBallOnShootButtonPressed;
    }

    private void ShootBallOnShootButtonPressed()
    {
        Vector3 spawnPosition = mainCam.transform.position + mainCam.transform.forward * 0.1f;
        Quaternion spawnRotation = mainCam.transform.rotation;

        GameObject spawnedBall = Instantiate(ballPrefab, spawnPosition, spawnRotation);
        Rigidbody rb = spawnedBall.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.AddForce(mainCam.transform.forward * ballForwardForce);
        }

        Destroy(spawnedBall, 5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
