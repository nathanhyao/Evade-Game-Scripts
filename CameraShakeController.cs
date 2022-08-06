using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraShakeController : MonoBehaviour
{
    [SerializeField] private AILocomotion aiScript;

    [Header("Camera Shake")]
    [SerializeField] private float magnitude;
    [SerializeField] private float roughness;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;

    [Header("Magnitude by Distance")]
    [SerializeField] private int maxDistance;
    private float distanceMultiplier;
    private float maxTime = 0.5f;
    private float distanceCheckTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        aiScript = GetComponent<AILocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceCheckTimer -= Time.deltaTime;
        if (distanceCheckTimer < 0.0f)
        {
            // distanceMultiplier exists between 0 (agent far from player) and 1 (close to player)
            distanceMultiplier = (maxDistance - aiScript.playerToAgent.magnitude) / maxDistance;

            if (distanceMultiplier < 0.0f)
                distanceMultiplier = 0.0f;

            distanceCheckTimer = maxTime;
        }
    }

    // Animation Events

    public void ShakeCamLight()
    {
        CameraShaker.Instance.ShakeOnce(magnitude * distanceMultiplier, roughness, fadeInTime, fadeOutTime);
    }

    public void ShakeCamHeavy()
    {
        CameraShaker.Instance.ShakeOnce(5.5f * magnitude * distanceMultiplier, roughness, fadeInTime, fadeOutTime);
    }
}
