using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class CameraShakeController : MonoBehaviour
{
    AILocomotion aiScript;

    [Header("Camera Shake")]
    public float magnitude;
    public float roughness;
    public float fadeInTime;
    public float fadeOutTime;

    private float distMultiplier;

    [Header("Magnitude by Distance")]
    public int maxDistance;
    private float maxTime = 0.5f;
    private float timer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        aiScript = GetComponent<AILocomotion>();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            // distanceMultiplier exists between 0 (agent far from player) and 1 (close to player)
            distMultiplier = (maxDistance - aiScript.playerToAgent.magnitude) / maxDistance;

            if (distMultiplier < 0.0f)
                distMultiplier = 0.0f;

            timer = maxTime;
        }
    }

    // Animation Events

    public void ShakeCamLight()
    {
        CameraShaker.Instance.ShakeOnce(magnitude * distMultiplier, roughness, fadeInTime, fadeOutTime);
    }

    public void ShakeCamHeavy()
    {
        CameraShaker.Instance.ShakeOnce(4.5f * magnitude * distMultiplier, roughness, fadeInTime, fadeOutTime);
    }
}
